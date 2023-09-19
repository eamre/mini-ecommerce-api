using ETicaretAPI.Application.Abstraction;
using ETicaretAPI.Application.Repositories;
using ETicaretAPI.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ETicaretAPI.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductWriteRepository _productWriteRepository;
        private readonly IProductReadRepository _productReadRepository;
        private readonly IOrderWriteRepository _orderWriteRepository;

        public ProductsController(IProductWriteRepository productWriteRepository, IProductReadRepository productReadRepository, IOrderWriteRepository orderWriteRepository)
        {
            _productWriteRepository = productWriteRepository;
            _productReadRepository = productReadRepository;
            _orderWriteRepository = orderWriteRepository;
        }
        string customerIdString = "3F2504E0-4F89-11D3-9A0C-0305E82C3301";
        [HttpGet]
        public async Task Get()
        {
            await _orderWriteRepository.AddAsync(new Order()
            {
                
                CustomerId = Guid.Parse(customerIdString),
                Description = "bla bla",
                Address = "ankara"
            });
            

            await _orderWriteRepository.SaveAsync();
        }
       
    }
}
