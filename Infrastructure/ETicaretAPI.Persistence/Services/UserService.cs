using ETicaretAPI.Application.Abstractions.Services;
using ETicaretAPI.Application.DTOs.User;
using ETicaretAPI.Application.Exceptions;
using ETicaretAPI.Application.Features.Commands.AppUsers.CreateUser;
using ETicaretAPI.Application.Helpers;
using ETicaretAPI.Application.Repositories.Endpoints;
using ETicaretAPI.Application.RequestParameters;
using ETicaretAPI.Domain.Entities;
using ETicaretAPI.Domain.Entities.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Server.IIS.Core;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Persistence.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IEndpointReadRepository _endpointReadRepository;

        public UserService(UserManager<AppUser> userManager, IEndpointReadRepository endpointReadRepository)
        {
            _userManager = userManager;
            _endpointReadRepository = endpointReadRepository;
        }

        public async Task<CreateUserResponse> CreateAsync(CreateUserRequest model)
        {
            IdentityResult result = await _userManager.CreateAsync(new()
            {
                Id = Guid.NewGuid().ToString(),
                NameSurname = model.Name,
                UserName = model.Username,
                Email = model.Email,
            }, model.Password);

            CreateUserResponse response = new() { Success = result.Succeeded };

            if (result.Succeeded)
                response.Message = "Kullanici Basariyla Olusturuldu.";

            else
                foreach (var err in result.Errors)
                {
                    response.Message += $"{err.Code} - {err.Description}\n";
                }

            return response;
        }

        public async Task UpdateRefreshTokenAsync(string refreshToken, AppUser user, DateTime accessTokenDate, int addOnAccessTokenLifeTime)
        {
            if (user != null)
            {
                user.RefreshToken = refreshToken;
                user.RefreshTokenEndDate = accessTokenDate.AddSeconds(addOnAccessTokenLifeTime);
                await _userManager.UpdateAsync(user);
            }
            else
                throw new NotFoundUserException();
        }

        public async Task UpdatePasswordAsync(string userId, string resetToken, string newPassword)
        {
            AppUser user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                resetToken = resetToken.UrlDecode();
                IdentityResult result = await _userManager.ResetPasswordAsync(user, resetToken, newPassword);
                if (result.Succeeded)
                    await _userManager.UpdateSecurityStampAsync(user);
                else
                    throw new PasswordChangeFailedException();
            }
        }

        public async Task<List<ListUser>> GetAllUsersAsync(Pagination pagination)
        {
            var users = await _userManager.Users
                .Skip(pagination.Size * pagination.Page)
                .Take(pagination.Size)
                .ToListAsync();

            return users.Select(user => new ListUser
            {
                Id = user.Id,
                Email = user.Email,
                NameSurname = user.NameSurname,
                TwoFactorEnabled = user.TwoFactorEnabled,
                UserName = user.UserName
            }).ToList();
        }

        public int TotalUsersCount => _userManager.Users.Count();

        public async Task AssignRoleToUserAsync(string userId, string[] roles)
        {
            AppUser user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                var userRoles = await _userManager.GetRolesAsync(user);
                await _userManager.RemoveFromRolesAsync(user, userRoles);

                await _userManager.AddToRolesAsync(user, roles);
            }
        }

        public async Task<string[]> GetRolesToUserAsync(string userIdOrName)
        {
            AppUser user = await _userManager.FindByIdAsync(userIdOrName);

            user ??= await _userManager.FindByNameAsync(userIdOrName);

            if (user != null)
            {
                var userRoles = await _userManager.GetRolesAsync(user);
                return userRoles.ToArray();
            }

            return null;
        }

        public async Task<bool> HasRolePermissionToEndpointAsync(string name, string code)
        {
            var userRoles = await GetRolesToUserAsync(name);

            if (!userRoles.Any())
                return false;

            Endpoint? endpoint = await _endpointReadRepository.Table
                .Include(e => e.Roles)
                .FirstOrDefaultAsync(e => e.Code == code);

            if (endpoint == null)
                return false;

            var endpointRoles = endpoint.Roles.Select(r => r.Name);

            foreach (var userRole in userRoles)
            {
                foreach (var endpointRole in endpointRoles)
                    if (userRole == endpointRole)
                        return true;

            }
            return false;
        }
    }
}
