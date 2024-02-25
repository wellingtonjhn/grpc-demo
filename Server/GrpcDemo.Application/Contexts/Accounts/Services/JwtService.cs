namespace GrpcDemo.Application.Contexts.Accounts.Services
{
    using System;
    using System.IdentityModel.Tokens.Jwt;
    using System.Security.Claims;
    using System.Security.Principal;
    using Contracts;
    using Microsoft.IdentityModel.Tokens;
    using Models;

    public class JwtService : IJwtService
    {
        private readonly JwtSettings _settings;

        public JwtService(JwtSettings settings) => _settings = settings;

        public JsonWebToken CreateJsonWebToken(Account user)
        {
            var identity = GetClaimsIdentity(user);
            var handler = new JwtSecurityTokenHandler();

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = identity,
                Issuer = _settings.Issuer,
                Audience = _settings.Audience,
                IssuedAt = _settings.IssuedAt,
                NotBefore = _settings.NotBefore,
                Expires = _settings.AccessTokenExpiration,
                SigningCredentials = _settings.SigningCredentials
            };

            var securityToken = handler.CreateToken(tokenDescriptor);

            var accessToken = handler.WriteToken(securityToken);

            return new JsonWebToken
            {
                AccessToken = accessToken,
                //RefreshToken = CreateRefreshToken(user.Email),
                ExpiresInSeconds = (long)TimeSpan.FromMinutes(_settings.ValidForMinutes).TotalSeconds,
                ExpirationDate = securityToken.ValidTo
            };
        }

        //private RefreshToken CreateRefreshToken(string username)
        //{
        //    var refreshToken = new RefreshToken
        //    {
        //        Username = username,
        //        ExpirationDate = _settings.RefreshTokenExpiration
        //    };

        //    string token;
        //    var randomNumber = new byte[32];

        //    using (var rng = RandomNumberGenerator.Create())
        //    {
        //        rng.GetBytes(randomNumber);
        //        token = Convert.ToBase64String(randomNumber);
        //    }

        //    refreshToken.Token = token.Replace("+", string.Empty)
        //        .Replace("=", string.Empty)
        //        .Replace("/", string.Empty);

        //    return refreshToken;
        //}

        private static ClaimsIdentity GetClaimsIdentity(Account user)
        {
            var identity = new ClaimsIdentity
            (
                new GenericIdentity(user.Email),
                new[] {
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Sub, user.Name),
                    new Claim(JwtRegisteredClaimNames.Email, user.Email)
                }
            );

            return identity;
        }
    }

}