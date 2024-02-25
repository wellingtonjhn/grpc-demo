namespace GrpcDemo.Client
{
    using System;
    using System.CodeDom.Compiler;
    using System.IO;
    using System.Text;
    using YamlDotNet.Serialization;

    public static class Extensions
    {

        public static bool IsNull(this object value) => value is null;

        public static void Dump(this object value)
        {
            if (value.IsNull())
                return;

            var builder = new StringBuilder();
            SerializeYaml(value, builder);

            Console.WriteLine(builder);
        }

        private static void SerializeYaml(object value, StringBuilder builder)
        {
            var serializer = new Serializer();
            serializer.Serialize(new IndentedTextWriter(new StringWriter(builder), "-"), value);
        }
    }
}