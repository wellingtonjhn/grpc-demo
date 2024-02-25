namespace GrpcDemo.Application.Contexts.Accounts.Models
{
    using System.Collections.Generic;
    using Core;
    using Encription = BCrypt.Net.BCrypt;

    public class Password : ValueObject
    {
        private Password(string value) => Value = value;

        public string Value { get; private set; }

        public static Password Create(string value)
        {
            var hash = Encription.EnhancedHashPassword(value);
            return new Password(hash);
        }

        public bool Validate(string password) => Encription.EnhancedVerify(password, Value);

        public override string ToString() => Value;

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }

        public static implicit operator Password(string password) => new Password(password);
        public static implicit operator string(Password password) => password.Value;
    }
}