namespace GrpcDemo.Api.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.IO.Compression;
    using System.Runtime.InteropServices;
    using Grpc.AspNetCore.Server;
    using Interceptors;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Server.Kestrel.Core;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;

    public static class GrpcExtensions
    {
        /// <summary>
        /// Configure a HTTP/2 endpoint without TLS in development environment or in a OSX machine
        /// This is a known issue: <see cref="https://docs.microsoft.com/en-us/aspnet/core/grpc/troubleshoot?view=aspnetcore-3.0#unable-to-start-aspnet-core-grpc-app-on-macos"/>
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="port"></param>
        public static IWebHostBuilder SetupHttp2WithoutTls(this IWebHostBuilder builder, int port)
            => builder.ConfigureAppConfiguration((context, config) =>
            {
                if (context.HostingEnvironment.IsDevelopment() || RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    builder.ConfigureKestrel(options =>
                    {
                        options.ListenLocalhost(port, o => o.Protocols = HttpProtocols.Http2);
                    });
                }
            });

        /// <summary>
        /// Configure gRPC dependencies using the default 'services.AddGrpc()' method applying custom settings if specified
        /// </summary>
        /// <param name="services"></param>
        /// <param name="options">gRPC options configuration flags</param>
        /// <returns></returns>
        public static IServiceCollection AddGrpcDependencies(this IServiceCollection services, params GrpcOptions[] options)
        {
            services.AddGrpc(opt =>
            {
                var actions = new Dictionary<GrpcOptions, Action<GrpcServiceOptions>>
                {
                    { GrpcOptions.EnableDetailedErrors, o => o.EnableDetailedErrors = true },
                    { GrpcOptions.EnableOptimalResponseCompression, o => o.ResponseCompressionLevel = CompressionLevel.Optimal },
                    { GrpcOptions.EnableExceptionHandlerInterceptor, o => o.Interceptors.Add<GrpcExceptionHandlerInterceptor>() },
                    { GrpcOptions.EnableRequestLoggerInterceptor, o => o.Interceptors.Add<GrpcRequestLoggerInterceptor>() },
                    { GrpcOptions.EnableHealthCheck, o =>
                    {
                        // TODO: to be implemented
                    } }
                };

                foreach (var option in options)
                {
                    var action = actions[option];
                    action(opt);
                }

                //options.MaxReceiveMessageSize = 1 * 1024 * 1024; // 1 megabytes
                //options.MaxSendMessageSize = 5 * 1024 * 1024; // 5 megabytes
            });

            return services;
        }
    }
}