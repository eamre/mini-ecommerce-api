using ETicaretAPI.Application.DTOs.Order;
using ETicaretAPI.Application.RequestParameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Application.Abstractions.Services
{
    public interface IOrderService
    {
        Task CreateOrderAsync(CreateOrder createOrder);
        Task<ListOrder> GetAllOrdersAsync(Pagination pagination);
        Task<SingleOrder> GetOrderByIdAsync(string id);
        Task<(bool, CompletedOrderDTO)> CompleteOrderAsync(string id);
    }
}
