namespace GrpcDemo.Infrastructure.Database
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading;
    using System.Threading.Tasks;
    using Application.Core;
    using Application.Extensions;
    using Coravel.Queuing.Interfaces;
    using MediatR;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;
    using Raven.Client.Documents;
    using Raven.Client.Documents.Indexes;
    using Raven.Client.Documents.Session;

    public class RavenContext : IRavenContext
    {
        private readonly IAsyncDocumentSession _session;
        private readonly ILogger<RavenContext> _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public RavenContext(
            IAsyncDocumentSession session,
            IMediator mediator,
            IQueue queue,
            ILogger<RavenContext> logger,
            IServiceScopeFactory serviceScopeFactory)
        {
            _session = session;
            _logger = logger;
            _serviceScopeFactory = serviceScopeFactory;

            _session.Advanced.OnBeforeStore += OnOnBeforeStore();
            _session.Advanced.OnAfterSaveChanges += OnAfterSaveChanges(queue);
        }

        public async Task<T> GetAsync<T>(
            string id,
            CancellationToken cancellationToken = default)
        {
            return await _session
                .LoadAsync<T>(id, cancellationToken)
                .ConfigureAwait(false);
        }

        public async Task<T> GetAsync<T>(
            Expression<Func<T, bool>> predicate,
            CancellationToken cancellationToken = default)
        {
            return await _session.Query<T>()
                .SingleOrDefaultAsync(predicate, cancellationToken)
                .ConfigureAwait(false);
        }

        public async Task<T> GetAsync<T, TIndex>(
            Expression<Func<T, bool>> expression,
            CancellationToken cancellationToken = default)
            where TIndex : AbstractIndexCreationTask
        {
            return await _session.Query<T>(typeof(TIndex).Name)
                .SingleOrDefaultAsync(expression, cancellationToken)
                .ConfigureAwait(false);
        }

        public async Task StoreAsync<T>(T item, CancellationToken cancellationToken = default)
        {
            if (item is null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            await _session
                .StoreAsync(item, cancellationToken)
                .ConfigureAwait(false);
        }

        public async Task SaveAsync(CancellationToken cancellationToken = default)
        {
            await _session
                .SaveChangesAsync(cancellationToken)
                .ConfigureAwait(false);
        }

        public void Delete<T>(T item)
        {
            if (item is null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            _session.Delete(item);
        }

        private static EventHandler<BeforeStoreEventArgs> OnOnBeforeStore()
        {
            return (sender, args) =>
            {
                var entity = (Entity)args.Entity;

                if (entity.Valid)
                    return;

                var notifications = entity.Notifications.GetMessages();
                var message = JsonConvert.SerializeObject(notifications);

                throw new InvalidOperationException(message);
            };
        }

        private EventHandler<AfterSaveChangesEventArgs> OnAfterSaveChanges(IQueue queue)
        {
            return (sender, args) =>
            {
                var savedEntity = (Entity)args.Entity;

                // Uses Coravel to enqueue an operation (in-memory) to be processed later as a background task
                queue.QueueAsyncTask(async () =>
                {
                    await DispatchDomainEventsAsync(savedEntity).ConfigureAwait(false);
                });
            };
        }

        private async Task DispatchDomainEventsAsync(Entity entity)
        {
            if (!entity.Events.Any())
                return;

            try
            {
                using var scope = _serviceScopeFactory.CreateScope();
                var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

                foreach (var @event in entity.Events)
                {
                    await mediator.Publish(@event).ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
        }
    }
}