namespace GrpcDemo.Application.Core
{
    using System;
    using MediatR;

    public abstract class Event : INotification
    {
        public DateTimeOffset CreatedAt = DateTimeOffset.UtcNow;
    }
}