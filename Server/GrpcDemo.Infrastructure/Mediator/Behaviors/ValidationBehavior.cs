namespace GrpcDemo.Infrastructure.Mediator.Behaviors
{
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Exceptions;
    using Flunt.Notifications;
    using MediatR;

    public sealed class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        public async Task<TResponse> Handle(
            TRequest request, 
            CancellationToken cancellationToken, 
            RequestHandlerDelegate<TResponse> next)
        {
            if (request is Notifiable notifiableRequest 
                && notifiableRequest.Invalid)
            {
                var validationErrors = notifiableRequest.Notifications
                    .Select(notification => new ValidationError(notification.Property, notification.Message))
                    .ToList();

                throw new RequestValidationException(validationErrors);
            }

            return await next().ConfigureAwait(false);
        }
    }
}