namespace GrpcDemo.Client
{
    using System;
    using System.Threading.Tasks;
    using Api.Proto;
    using Grpc.Core;
    using Grpc.Net.Client;

    public class AccountServiceGateway
    {
        private readonly AccountService.AccountServiceClient _client;

        public AccountServiceGateway(string address)
        {
            // Enable support for unencrypted HTTP2 (to be used when TLS is disabled on server)
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);

            var channel = GrpcChannel.ForAddress(address);

            _client = new AccountService.AccountServiceClient(channel);
        }

        /// <summary>
        /// Change account password
        /// </summary>
        /// <param name="accessToken">Requires authentication token</param>
        /// <param name="email">Account email</param>
        /// <param name="password">New password</param>
        /// <param name="passwordConfirmation">New password confirmation</param>
        /// <returns></returns>
        public async Task ChangePassword(
            string accessToken,
            string email,
            string password,
            string passwordConfirmation)
        {
            var auth = new Metadata
            {
                { "Authorization", $"Bearer {accessToken}" }
            };

            var response = await _client.ChangePasswordAsync(
                request: new ChangePasswordRequest
                {
                    Email = email,
                    Password = password,
                    PasswordConfirmation = passwordConfirmation
                }, 
                headers: auth);

            response.Dump();
        }

        /// <summary>
        /// Get an access token
        /// </summary>
        /// <param name="email">Account email</param>
        /// <param name="password">Account password</param>
        /// <returns></returns>
        public async Task<string> Login(string email, string password)
        {
            var response = await _client.LoginAsync(
                request: new LoginRequest
                {
                    Email = email,
                    Password = password
                });

            response.Dump();

            return response.AccessToken;
        }
    }
}