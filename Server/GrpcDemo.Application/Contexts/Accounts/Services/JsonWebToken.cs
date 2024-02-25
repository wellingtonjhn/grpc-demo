namespace GrpcDemo.Application.Contexts.Accounts.Services
{
    using System;

    public class JsonWebToken
    {
        public string AccessToken { get; set; }
        //public RefreshToken RefreshToken { get; set; }
        public string TokenType { get; set; } = "bearer";
        public long ExpiresInSeconds { get; set; }
        public DateTime ExpirationDate { get; set; }
    }
}