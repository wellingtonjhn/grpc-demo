namespace GrpcDemo.Infrastructure
{
    using System;
    using System.Reflection;
    using Application.Contexts.Accounts.Services;
    using Application.Core;
    using Coravel;
    using Database;
    using Database.Converters;
    using Mediator.Behaviors;
    using MediatR;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Newtonsoft.Json;
    using Raven.DependencyInjection;
    using Raven.Embedded;

    public static class Initializer
    {
        private static readonly Assembly ApplicationAssembly = Assembly.Load("GrpcDemo.Application");
        private static readonly Assembly InfrastructureAssembly = Assembly.Load("GrpcDemo.Infrastructure");

        public static IServiceCollection AddApplicationDependencies(this IServiceCollection services)
            => services
                .AddQueue()
                .AddRavenDb()
                .AddRepositories()
                .AddServices()
                .AddMediator();

        private static IServiceCollection AddMediator(this IServiceCollection services)
        {
            services.AddMediatR(ApplicationAssembly);
            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

            return services;
        }

        private static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            return services
                .AddScannedDependencies(InfrastructureAssembly, ServiceLifetime.Scoped, typeof(IRepository));
        }

        private static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddSingleton<JwtSettings>();
            services.AddScannedDependencies(ApplicationAssembly, ServiceLifetime.Scoped, typeof(IService));

            return services;
        }

        private static IServiceCollection AddRavenDb(this IServiceCollection services)
        {
            var configuration = services.BuildServiceProvider().GetRequiredService<IConfiguration>();

            var settings = ConfigureEmbeddedRavenDbServer(configuration);

            services
                .AddRavenDbDocStore(options =>
                {
                    options.Settings.DatabaseName = settings.DatabaseName;
                    options.Settings.Urls = settings.Urls;

                    options.BeforeInitializeDocStore = store =>
                    {
                        store.Conventions.JsonContractResolver = new RavenContractResolver();
                        store.Conventions.CustomizeJsonSerializer = serializer =>
                        {
                            serializer.ReferenceLoopHandling = ReferenceLoopHandling.Serialize;
                            serializer.Converters.Add(new EmailConverter());
                            serializer.Converters.Add(new PasswordConverter());
                        };
                    };
                })
                .AddRavenDbAsyncSession()
                .AddScoped<IRavenContext, RavenContext>();

            return services;
        }

        /// <summary>
        /// Configures a embedded RavenDb server
        /// <para>In a real application use an cloud instance instead, as https://cloud.ravendb.net</para>
        /// </summary>
        /// <returns>Server Settings</returns>
        private static RavenSettings ConfigureEmbeddedRavenDbServer(IConfiguration configuration)
        {
            const string databaseName = "GrpcDemo";

            EmbeddedServer.Instance.StartServer();
            EmbeddedServer.Instance.GetDocumentStore(databaseName); // when calling document store the database is created

            var shouldOpenRavenDbStudio = configuration.GetValue<bool>("OpenRavenDbStudioInBrowser");

            if (shouldOpenRavenDbStudio)
            {
                EmbeddedServer.Instance.OpenStudioInBrowser();
            }

            var serverUrl = EmbeddedServer.Instance.GetServerUriAsync().Result.AbsoluteUri;

            return new RavenSettings
            {
                Urls = new[] { serverUrl },
                DatabaseName = databaseName
            };
        }

        private static IServiceCollection AddScannedDependencies(
            this IServiceCollection services,
            Assembly assembly,
            ServiceLifetime lifetime,
            params Type[] types)
            => services.Scan(config =>
            {
                config.FromAssemblies(assembly)
                    .AddClasses(filter => filter.AssignableToAny(types))
                    .AsImplementedInterfaces()
                    .WithLifetime(lifetime);
            });
    }
}