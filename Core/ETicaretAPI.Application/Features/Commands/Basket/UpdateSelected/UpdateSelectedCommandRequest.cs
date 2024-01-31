using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Application.Features.Commands.Basket.UpdateSelected
{
    public class UpdateSelectedCommandRequest:IRequest<UpdateSelectedCommandResponse>
    {
        public string BasketItemId { get; set; }
        public bool IsSelected { get; set; }
    }
}
