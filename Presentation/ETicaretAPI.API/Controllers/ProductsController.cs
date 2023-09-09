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

        public ProductsController(IProductWriteRepository productWriteRepository, IProductReadRepository productReadRepository)
        {
            _productWriteRepository = productWriteRepository;
            _productReadRepository = productReadRepository;
        }

        [HttpGet]
        public async Task Get()
        {
            //await _productWriteRepository.AddRangeAsync(new()
            //{
            //    new()
            //    {
            //        Id = Guid.NewGuid(),
            //        Name = "Product1",
            //        Price = 100,
            //        CreateDate = DateTime.Now.ToUniversalTime(),
            //        Stock = 10
            //    },
            //    new()
            //    {
            //        Id = Guid.NewGuid(),
            //        Name = "Product2",
            //        Price = 200,
            //        CreateDate = DateTime.Now.ToUniversalTime(),
            //        Stock = 20
            //    },
            //    new()
            //    {
            //        Id = Guid.NewGuid(),
            //        Name = "Product3",
            //        Price = 300,
            //        CreateDate = DateTime.Now.ToUniversalTime(),
            //        Stock = 30
            //    },
            //});

            //await _productWriteRepository.SaveAsync();

            Product p = await _productReadRepository.GetByIdAsync("b62f460f-efbe-4b75-9baa-287499b29ae0");
            p.Name = "b";
            await _productWriteRepository.SaveAsync();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            Product product = await _productReadRepository.GetByIdAsync(id);
            return Ok(product);
        }
    }
}
