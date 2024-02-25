namespace GrpcDemo.Infrastructure.Database
{
    using System;
    using System.Linq.Expressions;
    using System.Threading;
    using System.Threading.Tasks;
    using Raven.Client.Documents.Indexes;

    public interface IRavenContext
    {
        Task<T> GetAsync<T>(
            string id,
            CancellationToken cancellationToken = default);

        Task<T> GetAsync<T>(
            Expression<Func<T, bool>> predicate,
            CancellationToken cancellationToken = default);

        Task<T> GetAsync<T, TIndex>(
            Expression<Func<T, bool>> expression,
            CancellationToken cancellationToken = default)
            where TIndex : AbstractIndexCreationTask;

        Task StoreAsync<T>(T item, CancellationToken cancellationToken = default);
        Task SaveAsync(CancellationToken cancellationToken = default);
        void Delete<T>(T item);
    }
}