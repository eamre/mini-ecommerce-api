using ETicaretAPI.Application.Abstractions.Services;
using ETicaretAPI.Application.DTOs.User;
using ETicaretAPI.Application.Exceptions;
using ETicaretAPI.Domain.Entities.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Application.Features.Commands.AppUsers.CreateUser
{
    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommandRequest, CreateUserCommandResponse>
    {
        private readonly IUserService _userService;

        public CreateUserCommandHandler(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<CreateUserCommandResponse> Handle(CreateUserCommandRequest request, CancellationToken cancellationToken)
        {
            var createUserResponse = await _userService.CreateAsync(new CreateUserRequest()
            {
                Email = request.Email,
                Name = request.Name,
                Username = request.Username,
                Password = request.Password,
                PasswordAgain = request.PasswordAgain
            });

            return new CreateUserCommandResponse()
            {
                Message = createUserResponse.Message,
                Success = createUserResponse.Success,
            };

            //throw new UserCreateFailedException();
        }
    }
}
