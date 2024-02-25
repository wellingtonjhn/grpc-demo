namespace GrpcDemo.Api.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Application.Core;
    using FluentResults;
    using Grpc.Core;

    public static class ResultExtensions
    {
        public static TResponse ToRpcResponse<T, TResponse>(this Result<T> result, Func<T, TResponse> func)
        {
            return result.IsSuccess && result.Value != null
                ? func(result.Value)
                : ToRpcResponse(result.ToResult(), default(Func<TResponse>));
        }

        public static TResponse ToRpcResponse<TResponse>(this Result result, Func<TResponse> func)
        {
            if (result.IsFailed)
            {
                ThrowRpcException(result);
            }

            return func();
        }

        private static void ThrowRpcException(ResultBase result)
        {
            var reason = result.Errors.Find(err => err.Metadata.ContainsKey(Errors.MessageTypeMetadataName));

            if (reason is null)
            {
                var genericError = result.Errors.FirstOrDefault()?.Message ?? "Unknown error";
                throw new RpcException(new Status(StatusCode.Unknown, genericError));
            }

            var status = MapStatusCode(reason.Metadata);
            throw new RpcException(new Status(status, reason.Message));
        }

        private static StatusCode MapStatusCode(IReadOnlyDictionary<string, object> metadata)
        {
            metadata.TryGetValue(Errors.MessageTypeMetadataName, out var resultStatus);

            if (!Enum.TryParse(resultStatus?.ToString(), out StatusCode status))
            {
                status = StatusCode.Unknown;
            }

            return status;
        }
    }
}