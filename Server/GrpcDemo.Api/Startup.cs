namespace GrpcDemo.Api
{
    using Extensions;
    using Infrastructure;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Services;

    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddApplicationDependencies();
            services.AddJwtAuthorization();

            //services.AddGrpc(); // see the AddGrpcDependencies extension method that has custom options

            services.AddGrpcDependencies(
                GrpcOptions.EnableDetailedErrors,
                GrpcOptions.EnableExceptionHandlerInterceptor,
                GrpcOptions.EnableRequestLoggerInterceptor,
                GrpcOptions.EnableOptimalResponseCompression);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGrpcService<GreeterService>();
                endpoints.MapGrpcService<AccountService>().RequireAuthorization();

                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");
                });
            });
        }
    }
}