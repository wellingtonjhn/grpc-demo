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

    public sealed class ChangePassword : Notifiable, IRequest<Result>
    {
        public string Email { get; }
        public string Password { get; }
        public string PasswordConfirmation { get; }

        public ChangePassword(string email, string password, string passwordConfirmation)
        {
            AddNotifications(new Contract()
                .IsEmail(email, nameof(email), "Invalid email address")
                .IsNotNullOrEmpty(password, nameof(password), "The password cannot be empty")
                .IsNotNullOrEmpty(passwordConfirmation, nameof(passwordConfirmation), "The password confirmation cannot be empty")
            );

            Email = email;
            Password = password;
            PasswordConfirmation = passwordConfirmation;
        }

        internal sealed class ChangePasswordHandler : IRequestHandler<ChangePassword, Result>
        {
            private readonly IAccountsRepository _repository;

            public ChangePasswordHandler(IAccountsRepository repository) => _repository = repository;

            public async Task<Result> Handle(ChangePassword request, CancellationToken cancellationToken)
            {
                var account = await _repository.GetByEmail(request.Email);

                if (account is null)
                {
                    return Result.Fail(Errors.NotFound("The specified account was not found"));
                }

                account.ChangePassword(request.Password, request.PasswordConfirmation);

                await _repository.Save(account);

                return Result.Ok();
            }
        }
    }
}