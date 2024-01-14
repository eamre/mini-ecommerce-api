using ETicaretAPI.Application.Features.Commands.AuthorizationEndpoints.AssignRoleEndpoint;
using ETicaretAPI.Application.Features.Queries.AuthorizationEndpoints.GetRolesToEndpoint;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ETicaretAPI.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorizationEndpointsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AuthorizationEndpointsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("get-roles-to-endpoint")]
        public async Task<IActionResult> GetRolesToEndpoints(GetRolesToEndpointQueryRequest request)
        {
            GetRolesToEndpointQueryResponse response = await _mediator.Send(request);
            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> AssignRole([FromBody]AssignRoleEndpointCommandRequest request)
        {
            request.Type = typeof(Program);
            AssignRoleEndpointCommandResponse response = await _mediator.Send(request);
            return Ok(response);
        }
    }
}
