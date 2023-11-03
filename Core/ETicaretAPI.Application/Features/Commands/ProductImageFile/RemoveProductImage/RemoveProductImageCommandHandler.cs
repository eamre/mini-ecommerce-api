using ETicaretAPI.Application.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace ETicaretAPI.Application.Features.Commands.ProductImageFile.RemoveProductImage
{
    public class RemoveProductImageCommandHandler : IRequestHandler<RemoveProductImageCommandRequest, RemoveProductImageCommandResponse>
    {
        private readonly IProductWriteRepository _productWriteRepository;
        private readonly IProductReadRepository _productReadRepository;

        public RemoveProductImageCommandHandler(IProductWriteRepository productWriteRepository, IProductReadRepository productReadRepository)
        {
            _productWriteRepository = productWriteRepository;
            _productReadRepository = productReadRepository;
        }

        public async Task<RemoveProductImageCommandResponse> Handle(RemoveProductImageCommandRequest request, CancellationToken cancellationToken)
        {
            var product = await _productReadRepository.Table.Include(p => p.ProductImages)
            .FirstOrDefaultAsync(p => p.Id == Guid.Parse(request.ProductId));

            var productImage = product?.ProductImages
                .FirstOrDefault(p => p.Id == Guid.Parse(request.ImageId));

            if (productImage != null)
                product?.ProductImages.Remove(productImage);

            await _productWriteRepository.SaveAsync();
            return new();
        }
    }
}
