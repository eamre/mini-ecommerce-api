using ETicaretAPI.Application.Features.Commands.Basket.AddItemToBasket;
using ETicaretAPI.Application.Features.Commands.Basket.RemoveBasketItem;
using ETicaretAPI.Application.Features.Commands.Basket.UpdateQuantity;
using ETicaretAPI.Application.Features.Queries.Basket.GetBasketItems;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ETicaretAPI.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "Admin")]
    public class BasketsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public BasketsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetBasketItems([FromQuery]GetBasketItemsQueryRequest basketItemsQueryRequest)
        {
            List<GetBasketItemsQueryResponse> response = await _mediator.Send(basketItemsQueryRequest);
            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> AddItemToBasket(AddItemToBasketCommandRequest itemToBasketRequest)
        {
            AddItemToBasketCommandResponse response = await _mediator.Send(itemToBasketRequest);
            return Ok(response);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateQuantity(UpdateQuantityCommandRequest quantityRequest)
        {
            UpdateQuantityCommandResponse response = await _mediator.Send(quantityRequest);
            return Ok(response);
        }

        [HttpDelete("{BasketItemId}")]
        public async Task<IActionResult> RemoveBasketItem([FromRoute]RemoveBasketItemCommandRequest removeBasketItem)
        {
            RemoveBasketItemCommandResponse response = await _mediator.Send(removeBasketItem);
            return Ok(response);
        }
    }
}
