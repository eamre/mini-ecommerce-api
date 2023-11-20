﻿using ETicaretAPI.Application.Abstractions.Services;
using ETicaretAPI.Application.Abstractions.Tokens;
using ETicaretAPI.Application.DTOs;
using ETicaretAPI.Application.DTOs.Facebook;
using ETicaretAPI.Application.Exceptions;
using ETicaretAPI.Application.Features.Commands.AppUsers.LoginUser;
using ETicaretAPI.Domain.Entities.Identity;
using Google.Apis.Auth;
using Google.Apis.Auth.OAuth2;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ETicaretAPI.Persistence.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ITokenHandler _tokenHandler;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public AuthService(UserManager<AppUser> userManager, ITokenHandler tokenHandler, IHttpClientFactory httpClientFactory, IConfiguration configuration, SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _tokenHandler = tokenHandler;
            _httpClient = httpClientFactory.CreateClient();
            _configuration = configuration;
            _signInManager = signInManager;
        }

        public async Task<Token> FacebookLoginAsync(string authToken, int accessTokenLifeTime)
        {
            string accessTokenResponse = await _httpClient.GetStringAsync($"https://graph.facebook.com/oauth/access_token?client_id={_configuration["ExternalLoginSettings:Facebook:ClientId"]}&client_secret={_configuration["ExternalLoginSettings:Facebook:ClientSecret"]}&grant_type=client_credentials");
            FacebookAccessTokenResponse? facebookAccessTokenResponse =
                JsonSerializer.Deserialize<FacebookAccessTokenResponse>(accessTokenResponse);
           
            string userAccessTokenValidation = await _httpClient.GetStringAsync($"https://graph.facebook.com/debug_token?input_token={authToken}&access_token={facebookAccessTokenResponse?.AccessToken}");
            FacebookUserAccessTokenValidation? validation =
                JsonSerializer.Deserialize<FacebookUserAccessTokenValidation>(userAccessTokenValidation);

            if (validation?.Data.IsValid != null)
            {
                string userInfoResponse =
                    await _httpClient.GetStringAsync($"https://graph.facebook.com/me?fields=email,name&access_token={authToken}");
                FacebookUserInfoResponse? userInfo = JsonSerializer.Deserialize<FacebookUserInfoResponse>(userInfoResponse);

                var loginInfo = new UserLoginInfo("FACEBOOK", validation.Data.UserId, "FACEBOOK");
                AppUser user = await _userManager.FindByLoginAsync(loginInfo.LoginProvider, loginInfo.ProviderKey);

                return await CreateUserExternalAsync(user, userInfo.Email, userInfo.Name, loginInfo, accessTokenLifeTime);
            }

            throw new Exception("Invalid external Authentication");
        }

        public async Task<Token> GoogleLoginAsync(string idToken, int accessTokenLifeTime)
        {
            var settings = new GoogleJsonWebSignature.ValidationSettings()
            {
                Audience = new List<string> { _configuration["ExternalLoginSettings:Google:ClientId"] }
            };

            var payload = await GoogleJsonWebSignature.ValidateAsync(idToken, settings); /*JWT’nin geçerli olup olmadığını kontrol ediyor. Payload, kullanıcıya ait bazı bilgileri içerir, örneğin e-posta, isim, resim, konu, vs*/
            var loginInfo = new UserLoginInfo("GOOGLE", payload.Subject, "GOOGLE");
            AppUser user = await _userManager.FindByLoginAsync(loginInfo.LoginProvider, loginInfo.ProviderKey);

            return await CreateUserExternalAsync(user, payload.Email, payload.Name, loginInfo, accessTokenLifeTime);
        }
    
        public async Task<Token> LoginAsync(string usernameOrEmail, string password, int accessTokenLifeTime)
        {
            AppUser user = await _userManager.FindByNameAsync(usernameOrEmail);

            if (user == null)
                user = await _userManager.Users.FirstOrDefaultAsync(u => u.Email == usernameOrEmail);
            if (user is null)
                throw new NotFoundUserException();

            SignInResult result = await _signInManager.CheckPasswordSignInAsync(user, password, false);

            if (result.Succeeded)
            {
                Token token = _tokenHandler.CreateAccessToken(accessTokenLifeTime);

                return token;
            }

            throw new AuthenticationErrorException();
        }
        
        private async Task<Token> CreateUserExternalAsync(AppUser user, string email, string name, UserLoginInfo loginInfo, int accessTokenLifeTime)
        {
            bool result = user != null;
            if (user == null)
            {
                user = await _userManager.FindByEmailAsync(email);
                if (user == null)
                {
                    user = new AppUser()
                    {
                        Id = Guid.NewGuid().ToString(),
                        Email = email,
                        UserName = email,
                        NameSurname = name,
                    };
                    var identityResult = await _userManager.CreateAsync(user);
                    result = identityResult.Succeeded;
                }
            }

            if (result)
            {
                await _userManager.AddLoginAsync(user, loginInfo);

                Token token = _tokenHandler.CreateAccessToken(accessTokenLifeTime);

                return token;
            }

            throw new Exception("Invalid external Authentication");
        }
    }
}