namespace GrpcDemo.Api.Services
{
    using System.Threading.Tasks;
    using Application.Contexts.Accounts.Features;
    using Extensions;
    using Google.Protobuf.WellKnownTypes;
    using Grpc.Core;
    using MediatR;
    using Microsoft.AspNetCore.Authorization;
    using Proto;

    public class AccountService : Proto.AccountService.AccountServiceBase
    {
        private readonly IMediator _mediator;

        public AccountService(IMediator mediator) => _mediator = mediator;

        [AllowAnonymous]
        public override async Task<LoginResponse> Login(LoginRequest request, ServerCallContext context)
        {
            var result = await _mediator
                .Send(new Authenticate(request.Email, request.Password))
                .ConfigureAwait(false);

            return result.ToRpcResponse(token =>
                new LoginResponse
                {
                    AccessToken = token.AccessToken,
                    ExpirationDate = Timestamp.FromDateTimeOffset(token.ExpirationDate)
                });
        }

        [AllowAnonymous]
        public override async Task<Empty> Create(CreateAccountRequest request, ServerCallContext context)
        {
            var result = await _mediator
                .Send(new CreateAccount(request.Name, request.Email, request.Password))
                .ConfigureAwait(false);

            return result.ToRpcResponse(() => new Empty());
        }

        public override async Task<Empty> ChangePassword(ChangePasswordRequest request, ServerCallContext context)
        {
            var result = await _mediator
                .Send(new ChangePassword(request.Email, request.Password, request.PasswordConfirmation))
                .ConfigureAwait(false);

            return result.ToRpcResponse(() => new Empty());
        }
    }
}