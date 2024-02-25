namespace GrpcDemo.Api.Extensions.Interceptors
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Grpc.Core;
    using Grpc.Core.Interceptors;
    using Infrastructure.Mediator.Exceptions;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;

    public class GrpcExceptionHandlerInterceptor : Interceptor
    {
        private readonly ILogger<GrpcExceptionHandlerInterceptor> _logger;

        public GrpcExceptionHandlerInterceptor(ILogger<GrpcExceptionHandlerInterceptor> logger) 
            => _logger = logger;

        public override Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
            TRequest request,
            ServerCallContext context,
            UnaryServerMethod<TRequest, TResponse> continuation)
            => TryExecute(() => continuation(request, context));

        public override Task<TResponse> ClientStreamingServerHandler<TRequest, TResponse>(
            IAsyncStreamReader<TRequest> requestStream,
            ServerCallContext context,
            ClientStreamingServerMethod<TRequest, TResponse> continuation)
            => TryExecute(async () => await base.ClientStreamingServerHandler(requestStream, context, continuation));

        public override Task ServerStreamingServerHandler<TRequest, TResponse>(
            TRequest request,
            IServerStreamWriter<TResponse> responseStream,
            ServerCallContext context,
            ServerStreamingServerMethod<TRequest, TResponse> continuation)
            => TryExecute(async () => await base.ServerStreamingServerHandler(request, responseStream, context, continuation));

        public override Task DuplexStreamingServerHandler<TRequest, TResponse>(
            IAsyncStreamReader<TRequest> requestStream,
            IServerStreamWriter<TResponse> responseStream,
            ServerCallContext context,
            DuplexStreamingServerMethod<TRequest, TResponse> continuation)
            => TryExecute(async () => await base.DuplexStreamingServerHandler(requestStream, responseStream, context, continuation));

        private async Task TryExecute(Func<Task> action)
        {
            try
            {
                await action().ConfigureAwait(false);
            }
            catch (RequestValidationException ex)
            {
                //ThrowInvalidArgumentsWithMetadata(ex);
                ThrowInvalidArguments(ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);

                throw new RpcException(new Status(StatusCode.Internal, ex.Message));
            }
        }

        private async Task<TResponse> TryExecute<TResponse>(Func<Task<TResponse>> action)
        {
            try
            {
                return await action().ConfigureAwait(false);
            }
            catch (RequestValidationException ex)
            {
                //ThrowInvalidArgumentsWithMetadata(ex);
                ThrowInvalidArguments(ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);

                throw new RpcException(new Status(StatusCode.Internal, ex.Message));
            }

            return default;
        }

        private static void ThrowInvalidArgumentsWithMetadata(RequestValidationException exception)
        {
            var metadata = new Metadata();

            foreach (var error in exception.Errors)
            {
                metadata.Add(new Metadata.Entry(error.Code, error.Message));
            }

            throw new RpcException(new Status(StatusCode.InvalidArgument, exception.Message), metadata);
        }

        private static void ThrowInvalidArguments(RequestValidationException exception)
        {
            var errors = exception.Errors.Select(a => a.Message);
            var message = JsonConvert.SerializeObject(errors);

            throw new RpcException(new Status(StatusCode.InvalidArgument, message));
        }
    }
}