namespace GrpcDemo.Infrastructure.Database
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using Raven.Client.Documents.Conventions;

    public class RavenContractResolver : DefaultRavenContractResolver
    {
        protected override List<MemberInfo> GetSerializableMembers(Type type)
        {
            var members = base.GetSerializableMembers(type);

            RemoveFields(members, "Events");
            RemoveIfIsNotifiableType(type, members);

            return members;
        }

        private static void RemoveIfIsNotifiableType(Type objectType, List<MemberInfo> members)
        {
            if (objectType.BaseType?.BaseType?.Name != "Notifiable")
                return;

            RemoveFields(members, "Valid", "Invalid", "Notifications");
        }

        private static void RemoveFields(List<MemberInfo> members, params string[] names)
        {
            foreach (var name in names)
            {
                members.RemoveAll(x => x.Name == name);
            }
        }
    }
}