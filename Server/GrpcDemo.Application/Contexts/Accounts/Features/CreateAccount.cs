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
    using Models;

    public sealed class CreateAccount : Notifiable, IRequest<Result>
    {
        public string Name { get; }
        public string Email { get; }
        public string Password { get; }

        public CreateAccount(string name, string email, string password)
        {
            AddNotifications(new Contract()
                .IsNotNullOrEmpty(name, nameof(name), "The name cannot be empty")
                .IsNotNullOrEmpty(password, nameof(password), "The password cannot be empty")
                .IsEmail(email, nameof(email), "Invalid email address")
            );

            Name = name;
            Email = email;
            Password = password;
        }

        internal sealed class CreateAccountHandler : IRequestHandler<CreateAccount, Result>
        {
            private readonly IAccountsRepository _repository;

            public CreateAccountHandler(IAccountsRepository repository) => _repository = repository;

            public async Task<Result> Handle(CreateAccount request, CancellationToken cancellationToken)
            {
                var currentAccount = await _repository.GetByEmail(request.Email);

                if (currentAccount != null)
                {
                    return Result.Fail(Errors.AlreadyExists("The specified email has already been registered"));
                }

                var account = new Account(request.Name, request.Email, request.Password);
                
                await _repository.Save(account);

                return Result.Ok();
            }
        }
    }
}