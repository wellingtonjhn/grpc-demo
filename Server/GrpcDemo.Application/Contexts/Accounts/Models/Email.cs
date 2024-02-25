namespace GrpcDemo.Application.Contexts.Accounts.Models
{
    using System.Collections.Generic;
    using Core;

    public sealed class Email : ValueObject
    {
        public string Value { get; }

        public Email(string value) => Value = value;

        public override string ToString() => Value;

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }

        public static implicit operator string(Email email) => email.Value;
        public static implicit operator Email(string email) => new Email(email);
    }
}