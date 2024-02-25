namespace GrpcDemo.Api.Extensions
{
    using System;

    [Flags]
    public enum GrpcOptions
    {
        EnableHealthCheck = 1,
        EnableOptimalResponseCompression = 2,
        EnableRequestLoggerInterceptor = 4,
        EnableExceptionHandlerInterceptor = 8,
        EnableDetailedErrors = 16
    }
}