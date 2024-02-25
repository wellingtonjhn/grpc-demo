namespace GrpcDemo.Application.Extensions
{
    using System.Collections.Generic;
    using System.Linq;
    using FluentResults;
    using Flunt.Notifications;
    using MediatR;

    public static class NotificationExtensions
    {
        public static IEnumerable<string> GetMessages(this IEnumerable<Notification> notifications) 
            => notifications.Select(n => n.Message);

        public static Result ToFailResult(this IEnumerable<Notification> notifications) 
            => ToFailResult<Unit>(notifications);

        public static Result<IEnumerable<T>> ToFailResult<T>(this IEnumerable<Notification> notifications)
        {
            var fails = notifications.Select(notification => Result.Fail<T>(notification.Message)).ToArray();
            return Result.Merge(fails);
        }
    }
}