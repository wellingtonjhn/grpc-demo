namespace GrpcDemo.Api
{
    using Extensions;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Hosting;

    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(builder =>
                {
                    builder
                        .SetupHttp2WithoutTls(port: 5000)
                        .UseStartup<Startup>()
                        .CaptureStartupErrors(true);
                });
    }
}
