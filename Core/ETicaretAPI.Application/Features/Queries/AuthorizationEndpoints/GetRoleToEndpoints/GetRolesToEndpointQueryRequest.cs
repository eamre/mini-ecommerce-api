using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Application.Features.Queries.AuthorizationEndpoints.GetRolesToEndpoint
{
    public class GetRolesToEndpointQueryRequest:IRequest<GetRolesToEndpointQueryResponse>
    {
        public string Menu { get; set; }
        public string Code { get; set; }
    }
}
