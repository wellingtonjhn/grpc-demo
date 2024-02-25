namespace GrpcDemo.Application.Contexts.Accounts.Events
{
    using System.Threading;
    using System.Threading.Tasks;
    using Core;
    using MediatR;
    using Microsoft.Extensions.Logging;

    public sealed class AccountCreated : Event
    {
        public string Name { get; }
        public string Email { get; }

        public AccountCreated(string name, string email)
        {
            Name = name;
            Email = email;
        }

        internal sealed class SendConfirmationEmailOnAccountCreated : INotificationHandler<AccountCreated>
        {
            private readonly ILogger<SendConfirmationEmailOnAccountCreated> _logger;

            public SendConfirmationEmailOnAccountCreated(ILogger<SendConfirmationEmailOnAccountCreated> logger)
            {
                _logger = logger;
            }

            public Task Handle(AccountCreated notification, CancellationToken cancellationToken)
            {
                // Confirm the operation sending an email to the customer (using an EmailService maybe)
                // or put it in a message broker to be processed later by another application
                // ... 

                _logger.LogInformation("New account was created with success", notification.Name, notification.Email);

                return Task.CompletedTask;
            }
        }
    }
}