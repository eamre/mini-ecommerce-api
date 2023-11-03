using ETicaretAPI.Application.Abstractions.Storage;
using ETicaretAPI.Application.Repositories;
using ETicaretAPI.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Application.Features.Commands.ProductImageFile.UploadProductImage
{
    public class UploadProductImageCommandHandler : IRequestHandler<UploadProductImageCommandRequest, UploadProductImageCommandResponse>
    {
        private readonly IProductReadRepository _productReadRepository;
        private readonly IStorageService _storageService;
        private readonly IProductImageFileWriteRepository _productImageFileWriteRepository;


        public UploadProductImageCommandHandler(IProductReadRepository productReadRepository, IStorageService storageService, IProductImageFileWriteRepository productImageFileWriteRepository)
        {
            _productReadRepository = productReadRepository;
            _storageService = storageService;
            _productImageFileWriteRepository = productImageFileWriteRepository;
        }

        public async Task<UploadProductImageCommandResponse> Handle(UploadProductImageCommandRequest request, CancellationToken cancellationToken)
        {
            var product = await _productReadRepository.GetByIdAsync(request.Id);

            var datas = await _storageService.UploadAsync("photo-images", request.FormFileCollections);
           
            await _productImageFileWriteRepository
                .AddRangeAsync(datas.Select(d => new Domain.Entities.ProductImageFile
                {
                    FileName = d.fileName,
                    Path = d.pathOrContainerName,
                    Storage = _storageService.StorageName,
                    Products = new List<Domain.Entities.Product> { product }
                }).ToList());
            await _productImageFileWriteRepository.SaveAsync();
            return new();
        }
    }
}
