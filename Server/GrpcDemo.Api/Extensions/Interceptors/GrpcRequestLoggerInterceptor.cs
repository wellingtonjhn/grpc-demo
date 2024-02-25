namespace GrpcDemo.Api.Extensions.Interceptors
{
    using System.Linq;
    using System.Threading.Tasks;
    using Grpc.Core;
    using Grpc.Core.Interceptors;
    using Microsoft.Extensions.Logging;

    public class GrpcRequestLoggerInterceptor : Interceptor
    {
        private readonly ILogger<GrpcRequestLoggerInterceptor> _logger;

        public GrpcRequestLoggerInterceptor(ILogger<GrpcRequestLoggerInterceptor> logger)
        {
            _logger = logger;
        }

        public override Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
            TRequest request,
            ServerCallContext context,
            UnaryServerMethod<TRequest, TResponse> continuation)
        {
            LogCall<TRequest, TResponse>(MethodType.Unary, context);
            return continuation(request, context);
        }

        public override Task<TResponse> ClientStreamingServerHandler<TRequest, TResponse>(
            IAsyncStreamReader<TRequest> requestStream,
            ServerCallContext context,
            ClientStreamingServerMethod<TRequest, TResponse> continuation)
        {
            LogCall<TRequest, TResponse>(MethodType.ClientStreaming, context);
            return base.ClientStreamingServerHandler(requestStream, context, continuation);
        }

        public override Task ServerStreamingServerHandler<TRequest, TResponse>(
            TRequest request,
            IServerStreamWriter<TResponse> responseStream,
            ServerCallContext context,
            ServerStreamingServerMethod<TRequest, TResponse> continuation)
        {
            LogCall<TRequest, TResponse>(MethodType.ServerStreaming, context);
            return base.ServerStreamingServerHandler(request, responseStream, context, continuation);
        }

        public override Task DuplexStreamingServerHandler<TRequest, TResponse>(
            IAsyncStreamReader<TRequest> requestStream,
            IServerStreamWriter<TResponse> responseStream,
            ServerCallContext context,
            DuplexStreamingServerMethod<TRequest, TResponse> continuation)
        {
            LogCall<TRequest, TResponse>(MethodType.DuplexStreaming, context);
            return base.DuplexStreamingServerHandler(requestStream, responseStream, context, continuation);
        }

        private void LogCall<TRequest, TResponse>(MethodType methodType, ServerCallContext context)
            where TRequest : class
            where TResponse : class
        {
            _logger.LogWarning($"Starting call. Type: {methodType}. Request: {typeof(TRequest)}. Response: {typeof(TResponse)}");
            
            WriteMetadata(context.RequestHeaders, "Caller-User");
            WriteMetadata(context.RequestHeaders, "Caller-Machine");
            WriteMetadata(context.RequestHeaders, "Caller-OS");
            WriteMetadata(context.RequestHeaders, "X-Correlation-Id");

            void WriteMetadata(Metadata headers, string key)
            {
                var headerValue = headers.SingleOrDefault(h => h.Key == key)?.Value;
                _logger.LogInformation($"{key}: {headerValue ?? "(unknown)"}");
            }
        }
    }
}