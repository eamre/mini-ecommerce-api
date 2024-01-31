using ETicaretAPI.Application.Abstractions.Services;
using MediatR;
using MediatR.Pipeline;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Application.Features.Commands.Basket.UpdateSelected
{
    public class UpdateSelectedCommandHandler : IRequestHandler<UpdateSelectedCommandRequest, UpdateSelectedCommandResponse>
    {
        private readonly IBasketService _basketService;

        public UpdateSelectedCommandHandler(IBasketService basketService)
        {
            _basketService = basketService;
        }

        public async Task<UpdateSelectedCommandResponse> Handle(UpdateSelectedCommandRequest request, CancellationToken cancellationToken)
        {
            await _basketService.UpdateSelectedAsync(new()
            {
                BasketItemId = request.BasketItemId,
                IsSelected = request.IsSelected,
            });
            return new();

        }
    }
}
