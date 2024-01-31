using ETicaretAPI.Application.Abstractions.Services;
using ETicaretAPI.Application.DTOs.Order;
using ETicaretAPI.Application.Repositories;
using ETicaretAPI.Application.Repositories.BasketItems;
using ETicaretAPI.Application.Repositories.Baskets;
using ETicaretAPI.Application.Repositories.CompletedOrders;
using ETicaretAPI.Application.RequestParameters;
using ETicaretAPI.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Persistence.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderWriteRepository _orderWriteRepository;
        private readonly IOrderReadRepository _orderReadRepository;
        private readonly ICompletedOrderWriteRepository _completedOrderWriteRepository;
        private readonly ICompletedOrderReadRepository _completedOrderReadRepository;
        private readonly IBasketService _basketService;
        private readonly IBasketItemWriteRepository _basketItemWriteRepository;

        public OrderService(IOrderWriteRepository orderWriteRepository, IOrderReadRepository orderReadRepository, ICompletedOrderWriteRepository completedOrderWriteRepository, ICompletedOrderReadRepository completedOrderReadRepository, IBasketService basketService, IBasketItemWriteRepository basketItemWriteRepository)
        {
            _orderWriteRepository = orderWriteRepository;
            _orderReadRepository = orderReadRepository;
            _completedOrderWriteRepository = completedOrderWriteRepository;
            _completedOrderReadRepository = completedOrderReadRepository;
            _basketService = basketService;
            _basketItemWriteRepository = basketItemWriteRepository;
        }

        public async Task<(bool, CompletedOrderDTO)> CompleteOrderAsync(string id)
        {
            Order? order = await _orderReadRepository.Table
                .Include(o => o.Basket)
                .ThenInclude(b => b.User)
                .FirstOrDefaultAsync(o => o.Id == Guid.Parse(id));

            if (order != null)
            {
                await _completedOrderWriteRepository.AddAsync(new()
                {
                    OrderId = Guid.Parse(id),
                });
                return (await _completedOrderWriteRepository.SaveAsync()>0, new CompletedOrderDTO()
                {
                    OrderCode = order.OrderCode,
                    OrderDate = order.CreateDate,
                    UserName = order.Basket.User.UserName,
                    UserSurname = order.Basket.User.NameSurname,
                    Email = order.Basket.User.Email
                });
            }
            return (false,new());
        }

        public async Task CreateOrderAsync(CreateOrder createOrder)
        {
            var orderCode = CreateOrderCode();

            await _orderWriteRepository.AddAsync(new()
            {
                Address = createOrder.Address,
                Id = Guid.Parse(createOrder.BasketId),
                Description = createOrder.Description,
                OrderCode = orderCode,
            });

            await _orderWriteRepository.SaveAsync();
            await ChangeIdUnselectedBasketItemsAsync();
            
        }

        public async Task<ListOrder> GetAllOrdersAsync(Pagination pagination)
        {
            var query = _orderReadRepository.Table.Include(o => o.Basket)
                .ThenInclude(b => b.User)
                .ThenInclude(u => u.Baskets)
                .ThenInclude(b => b.BasketItems)
                .ThenInclude(bi => bi.Product)
                /*.Include(o => o.CompletedOrder)*/;

            var data = query.Skip(pagination.Page * pagination.Size)
                .Take(pagination.Size);

            var data2 = from order in data
                        join completedOrder in _completedOrderReadRepository.Table
                        on order.Id equals completedOrder.OrderId into co
                        from _co in co.DefaultIfEmpty()
                        select new
                        {
                            Id = order.Id,
                            OrderCode = order.OrderCode,
                            CreateDate = order.CreateDate,
                            Basket = order.Basket,
                            Completed = _co != null ? true:false
                        };

            return new()
            {
                TotalOrderCount = await query.CountAsync(),
                Orders = await data2.Select(o => new
                {
                    Id = o.Id,
                    CreateDate = o.CreateDate,
                    OrderCode = o.OrderCode,
                    TotalPrice = o.Basket.BasketItems.Sum(bi => bi.Product.Price * bi.Quantity),
                    UserName = o.Basket.User.UserName,
                    o.Completed
                    //Completed = o.CompletedOrder != null
                }).ToListAsync()
            };
        }

        public async Task<SingleOrder> GetOrderByIdAsync(string id)
        {
            var data = await _orderReadRepository.Table
                .Include(o => o.Basket)
                .ThenInclude(b => b.BasketItems)
                .ThenInclude(bi => bi.Product)
                .Include(o => o.CompletedOrder)
                .FirstOrDefaultAsync(o => o.Id == Guid.Parse(id));

            return new()
            {
                Id = data.Id.ToString(),
                Address = data.Address,
                CreatedDate = data.CreateDate,
                Description = data.Description,
                OrderCode = data.OrderCode,
                Completed = data.CompletedOrder != null,
                BasketItems = data.Basket.BasketItems.Select(bi => new
                {
                    bi.Product.Name,
                    bi.Product.Price,
                    bi.Quantity,
                })
            };
        }

        private string CreateOrderCode()
        {
            var orderCode = (new Random().NextDouble() * 10000).ToString();
            orderCode = orderCode.Substring(orderCode.IndexOf(',') + 1, orderCode.Length - orderCode.IndexOf(",") - 1);
            return orderCode;
        }

        private async Task ChangeIdUnselectedBasketItemsAsync()
        {
            var basketItems = await _basketService.GetBasketItemsAsync();

            var newBasketId = _basketService.GetUserActiveBasket.Id;

            foreach (var item in basketItems)
            {
                if ((bool)!item.IsSelected)
                {
                    item.BasketId = newBasketId;
                }
            }
            await _basketItemWriteRepository.SaveAsync();
        }
    }
}
