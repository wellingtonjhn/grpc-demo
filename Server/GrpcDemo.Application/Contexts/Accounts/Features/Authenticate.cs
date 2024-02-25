namespace GrpcDemo.Application.Contexts.Accounts.Features
{
    using System.Threading;
    using System.Threading.Tasks;
    using Contracts;
    using Core;
    using FluentResults;
    using Flunt.Notifications;
    using Flunt.Validations;
    using MediatR;
    using Services;

    public sealed class Authenticate : Notifiable, IRequest<Result<JsonWebToken>>
    {
        public string Email { get; }
        public string Password { get; }

        public Authenticate(string email, string password)
        {
            AddNotifications(new Contract()
                .IsEmail(email, nameof(email), "Invalid email address")
                .IsNotNullOrEmpty(password, nameof(password), "The password cannot be empty")
            );

            Email = email;
            Password = password;
        }

        internal sealed class AuthenticateUserAccountHandler : IRequestHandler<Authenticate, Result<JsonWebToken>>
        {
            private readonly IAccountsRepository _repository;
            private readonly IJwtService _jwtService;

            public AuthenticateUserAccountHandler(IAccountsRepository repository, IJwtService jwtService)
            {
                _repository = repository;
                _jwtService = jwtService;
            }

            public async Task<Result<JsonWebToken>> Handle(Authenticate request, CancellationToken cancellationToken)
            {
                var account = await _repository.GetByEmail(request.Email);

                if (account is null
                    || !account.Password.Validate(request.Password))
                {
                    return Result.Fail(Errors.PermissionDenied("Invalid email or password"));
                }

                var token = _jwtService.CreateJsonWebToken(account);

                return Result.Ok(token);
            }
        }
    }
}