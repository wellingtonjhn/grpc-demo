namespace GrpcDemo.Application.Core
{
    using FluentResults;

    public static class Errors
    {
        public enum ErrorCode
        {
            Cancelled,
            InvalidArgument,
            NotFound,
            AlreadyExists,
            PermissionDenied,
            Unauthenticated,
            OutOfRange,
            Unimplemented,
            Internal,
            Unavailable
        }

        public const string MessageTypeMetadataName = "message.type";

        public static Error Create(string message, ErrorCode code)
            => new Error(message).WithMetadata(MessageTypeMetadataName, code);

        public static Error AlreadyExists(string message) => Create(message, ErrorCode.AlreadyExists);
        public static Error NotFound(string message) => Create(message, ErrorCode.NotFound);
        public static Error Cancelled(string message) => Create(message, ErrorCode.Cancelled);
        public static Error InvalidArgument(string message) => Create(message, ErrorCode.InvalidArgument);
        public static Error PermissionDenied(string message) => Create(message, ErrorCode.PermissionDenied);
        public static Error Unauthenticated(string message) => Create(message, ErrorCode.Unauthenticated);
        public static Error OutOfRange(string message) => Create(message, ErrorCode.OutOfRange);
        public static Error Unimplemented(string message) => Create(message, ErrorCode.Unimplemented);
        public static Error Internal(string message) => Create(message, ErrorCode.Internal);
        public static Error Unavailable(string message) => Create(message, ErrorCode.Unavailable);
    }
}