using ETicaretAPI.Application.RequestParameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Application.Abstractions.Services
{
    public interface IRoleService
    {
        Task<(int totalRoleCount, Dictionary<string, string> roles)> GetAllRoles(Pagination pagination);
        Task<(string id, string name)> GetRoleById(string id);
        Task<bool> CreateRole(string name);
        Task<bool> UpdateRole(string id, string name);
        Task<bool> DeleteRole(string id);

    }
}
