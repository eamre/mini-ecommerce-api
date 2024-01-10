using ETicaretAPI.Application.Abstractions.Services;
using ETicaretAPI.Application.RequestParameters;
using ETicaretAPI.Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Persistence.Services
{
    public class RoleService : IRoleService
    {
        private readonly RoleManager<AppRole> _roleManager;

        public RoleService(RoleManager<AppRole> roleManager)
        {
            _roleManager = roleManager;
        }

        public async Task<bool> CreateRole(string name)
        {
            IdentityResult result = await _roleManager.CreateAsync(new()
            {
                Id = Guid.NewGuid().ToString(),
                Name = name,
            });

            return result.Succeeded;
        }

        public async Task<bool> DeleteRole(string id)
        {
            AppRole role = await _roleManager.FindByIdAsync(id);
            IdentityResult result = await _roleManager.DeleteAsync(role);
            return result.Succeeded;
        }

        public async Task<(int totalRoleCount, Dictionary<string, string> roles)> GetAllRoles(Pagination pagination)
        {
            int totalRoleCount = await _roleManager.Roles.CountAsync();

            Dictionary<string, string> roles = await _roleManager.Roles
                .Skip(pagination.Size * pagination.Page)
                .Take(pagination.Size)
                .ToDictionaryAsync(r => r.Id, r => r.Name);

            return new()
            {
                totalRoleCount = totalRoleCount,
                roles = roles
            };
        }

        public async Task<(string id, string name)> GetRoleById(string id)
        {
            AppRole role = await _roleManager.FindByIdAsync(id);
            return (role.Id, role.Name);
        }

        public async Task<bool> UpdateRole(string id, string name)
        {
            AppRole role = await _roleManager.FindByIdAsync(id);
            role.Name = name;
            IdentityResult result = await _roleManager.UpdateAsync(role);
            return result.Succeeded;
        }
    }
}
