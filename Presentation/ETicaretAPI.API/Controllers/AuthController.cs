using ETicaretAPI.Application.Features.Commands.AppUsers.FacebookLogin;
using ETicaretAPI.Application.Features.Commands.AppUsers.GoogleLogin;
using ETicaretAPI.Application.Features.Commands.AppUsers.LoginUser;
using ETicaretAPI.Application.Features.Commands.AppUsers.RefreshTokenLogin;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ETicaretAPI.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> RefreshTokenLogin([FromBody] RefreshTokenLoginCommandRequest refreshTokenLoginCommandRequest)
        {
            RefreshTokenLoginCommandResponse response = await _mediator.Send(refreshTokenLoginCommandRequest);
            return Ok(response);
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> Login([FromBody] LoginUserCommandRequest loginRequest)
        {
            var loginUserResponse = await _mediator.Send(loginRequest);
            return Ok(loginUserResponse);
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginCommandRequest googleLoginCommandRequest)
        {
            var response = await _mediator.Send(googleLoginCommandRequest);
            return Ok(response);
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> FacebookLogin([FromBody] FacebookLoginCommandRequest facebookLoginCommandRequest)
        {
            var response = await _mediator.Send(facebookLoginCommandRequest);
            return Ok(response);
        }
    }
}
