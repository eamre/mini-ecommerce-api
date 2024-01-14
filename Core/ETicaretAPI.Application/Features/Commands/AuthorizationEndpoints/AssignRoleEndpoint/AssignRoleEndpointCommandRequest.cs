using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Application.Features.Commands.AuthorizationEndpoints.AssignRoleEndpoint
{
    public class AssignRoleEndpointCommandRequest : IRequest<AssignRoleEndpointCommandResponse>
    {
        public string Menu { get; set; }
        public string[] Roles { get; set; }
        public string Code { get; set; }
        public Type? Type { get; set; }

    }
}
