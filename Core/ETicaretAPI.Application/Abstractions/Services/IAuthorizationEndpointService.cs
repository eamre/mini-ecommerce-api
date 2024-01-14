using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Application.Abstractions.Services
{
    public interface IAuthorizationEndpointService
    {
        public Task AssignRoleEndpointAsync(string menu, string[] roles, string code, Type type);
        public Task<List<string>> GetRolesToEndpointAsync(string code, string menu);
    }
}
