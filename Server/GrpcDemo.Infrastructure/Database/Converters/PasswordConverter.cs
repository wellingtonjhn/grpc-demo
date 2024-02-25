namespace GrpcDemo.Infrastructure.Database.Converters
{
    using System;
    using Application.Contexts.Accounts.Models;
    using Newtonsoft.Json;

    public class PasswordConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) => objectType == typeof(Password);

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return reader.Value == null
                ? null
                : (Password)reader.Value.ToString();
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var password = (value as Password)?.Value;
            writer.WriteValue(password);
        }
    }
}