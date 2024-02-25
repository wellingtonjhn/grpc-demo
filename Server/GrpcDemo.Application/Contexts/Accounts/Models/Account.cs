namespace GrpcDemo.Application.Contexts.Accounts.Models
{
    using Core;
    using Events;
    using Flunt.Validations;

    public sealed class Account : Entity
    {
        public string Name { get; private set; }
        public Email Email { get; private set; }
        public Password Password { get; private set; }

        // Protected constructor to be used when query data from db
        protected Account() { }

        public Account(string name, string email, string password)
        {
            Name = name;
            Email = email;
            Password = Password.Create(password);

            // Creates an internal domain event to be processed after store the Account on database
            // This domain event can handle other operations related with account creation
            RegisterEvent(new AccountCreated(name, email)); 
        }

        public void ChangePassword(string password, string passwordConfirmation)
        {
            AddNotifications(new Contract()
                .IsTrue(password.Equals(passwordConfirmation), nameof(password), "The passwords aren't equals")
                .HasMinLen(password, 8, nameof(password), "The password should have at least 8 characters")
            );

            Password = Password.Create(password);

            //RegisterEvent(new PasswordChanged(email));
        }
    }
}
