namespace GrpcDemo.Application.Core
{
    using System;
    using System.Collections.Generic;
    using Flunt.Notifications;

    public abstract class Entity : Notifiable
    {
        public string Id { get; protected set; } = string.Empty; // RavenDb identities are auto-generated as string
        public DateTime CreatedAt { get; protected set; } = DateTime.UtcNow;

        public ISet<Event> Events { get; } = new HashSet<Event>();

        /// <summary>
        /// Register a domain event to be processed after <see cref="Entity"/> store
        /// </summary>
        /// <param name="event"></param>
        public void RegisterEvent(Event @event)
            => Events.Add(@event);

        public static bool operator !=(Entity a, Entity b)
            => !(a == b);

        public override int GetHashCode()
            => GetType().GetHashCode() * 907 + Id.GetHashCode();

        public override string ToString()
            => $"{GetType().Name} [Id={Id}";

        public override bool Equals(object obj)
        {
            var compareTo = obj as Entity;

            if (ReferenceEquals(this, compareTo)) return true;
            if (ReferenceEquals(null, compareTo)) return false;

            return Id.Equals(compareTo.Id);
        }

        public static bool operator ==(Entity a, Entity b)
        {
            if (ReferenceEquals(a, null) && ReferenceEquals(b, null))
                return true;

            if (ReferenceEquals(a, null) || ReferenceEquals(b, null))
                return false;

            return a.Equals(b);
        }
    }
}