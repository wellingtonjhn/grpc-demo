namespace GrpcDemo.Infrastructure.Database.Converters
{
    using System;
    using Application.Contexts.Accounts.Models;
    using Newtonsoft.Json;

    public class EmailConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) => objectType == typeof(Email);

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return new Email(reader.Value?.ToString());
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var email = (value as Email)?.Value;
            writer.WriteValue(email);
        }
    }
}