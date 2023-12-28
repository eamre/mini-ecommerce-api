using ETicaretAPI.Application.Abstractions.Services;
using ETicaretAPI.Application.Features.Commands.AppUsers.CreateUser;
using ETicaretAPI.Application.Features.Commands.AppUsers.FacebookLogin;
using ETicaretAPI.Application.Features.Commands.AppUsers.GoogleLogin;
using ETicaretAPI.Application.Features.Commands.AppUsers.LoginUser;
using ETicaretAPI.Application.Features.Commands.AppUsers.UpdatePassword;
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
        private readonly IMailService _mailService;

        public UsersController(IMediator mediator, IMailService mailService)
        {
            _mediator = mediator;
            _mailService = mailService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserCommandRequest createRequest)
        {
            var createUserResponse = await _mediator.Send(createRequest);
            return Ok(createUserResponse);
        }

        [HttpGet]
        public async Task<IActionResult> MailTest()
        {
            await _mailService.SendMailAsync("lesson.12@yandex.com", "test", "<strong>bu test maili</strong>");
            return Ok();
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> UpdatePassword([FromBody] UpdatePasswordCommandRequest updatePasswordRequest)
        {
            var response = await _mediator.Send(updatePasswordRequest);
            return Ok(response);
        }
    }
}
