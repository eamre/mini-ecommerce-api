using ETicaretAPI.Application.ViewModels.Products;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Application.Validators.Products
{
    public class CreateProductValidator:AbstractValidator<VM_Create_Product>
    {
        public CreateProductValidator()
        {
            RuleFor(p => p.Name).NotEmpty()
                .NotNull()
                .WithMessage("Lütfen ürün adını boş geçmeyiniz")
                .MaximumLength(150)
                .MinimumLength(2)
                .WithMessage("Ürün adı 2 ile 150 karakter arasında yazın");

            RuleFor(p => p.Stock)
                .NotNull()
                .WithMessage("Lütfen stok bilgisini boş geçmeyiniz")
                .Must(s => s >= 0)
                .WithMessage("Stok bilgisi negatif olamaz");

            RuleFor(p => p.Price)
                .NotNull()
                .WithMessage("Lütfen fiyat alanını doldurun")
                .Must(p => p >= 0)
                .WithMessage("Fiyat bilgisi negatif olamaz");
        }
    }
}
