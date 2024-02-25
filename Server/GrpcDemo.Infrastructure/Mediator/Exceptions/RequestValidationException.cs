namespace GrpcDemo.Infrastructure.Mediator.Exceptions
{
    using System;
    using System.Collections.Generic;

    public readonly struct ValidationError
    {
        public string Code { get; }
        public string Message { get; }

        public ValidationError(string code, string message)
        {
            Code = code;
            Message = message;
        }
    }

    public class RequestValidationException : Exception
    {
        private readonly List<ValidationError> _errors = new List<ValidationError>();
        public IReadOnlyCollection<ValidationError> Errors => _errors;

        public RequestValidationException(IEnumerable<ValidationError> errors)
        {
            _errors.AddRange(errors);
        }
    }
}