using ETicaretAPI.Application.Features.Commands.AppUsers.CreateUser;
using ETicaretAPI.Application.Features.Commands.AppUsers.LoginUser;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ETicaretAPI.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IMediator _mediator;
        public UsersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserCommandRequest createRequest)
        {
            var createUserResponse = await _mediator.Send(createRequest);
            return Ok(createUserResponse);
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> Login([FromBody] LoginUserCommandRequest loginRequest)
        {
            var loginUserResponse = await _mediator.Send(loginRequest);
            return Ok(loginUserResponse);
        }
    }
}
