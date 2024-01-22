using ETicaretAPI.Application.DTOs;
using ETicaretAPI.Domain.Entities.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Application.Abstractions.Tokens
{
    public interface ITokenHandler
    {
        Task<Token> CreateAccessTokenAsync(int second, AppUser appUser);
        string CreateRefreshToken();
    }
}
