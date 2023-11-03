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
        private readonly UserManager<AppUser> _userManager;
        public CreateUserCommandHandler(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }
        public async Task<CreateUserCommandResponse> Handle(CreateUserCommandRequest request, CancellationToken cancellationToken)
        {
            IdentityResult result = await _userManager.CreateAsync(new()
            {
                Id = Guid.NewGuid().ToString(),
                NameSurname = request.Name,
                UserName = request.Username,
                Email = request.Email,               
            },request.Password);

            CreateUserCommandResponse response = new() { Success = result.Succeeded};

            if(result.Succeeded)
                response.Message = "Kullanici Basariyla Olusturuldu.";
            
            else
                foreach(var err in result.Errors)
                {
                    response.Message += $"{err.Code} - {err.Description}\n";
                }
            return response;
            //throw new UserCreateFailedException();
        }
    }
}
