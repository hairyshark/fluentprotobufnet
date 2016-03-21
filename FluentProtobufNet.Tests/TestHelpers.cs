using FluentProtobufNet.Helpers;

namespace FluentProtobufNet.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text;

    using NUnit.Framework;

    using ProtoBuf.Meta;

    public static class TestHelpers
    {
        public static T RoundTrip<T>(this T obj, RuntimeTypeModel model) where T : class
        {
            Assert.IsNotNull(obj, "obj");

            var schema = model.GetSchema(typeof(T));

            Assert.IsNotNullOrEmpty(schema);

            Debug.WriteLine(schema);

            using (var ms = new MemoryStream())
            {
                model.Serialize(ms, obj);

                Debug.WriteLine(Encoding.UTF8.GetString(ms.GetBuffer(), 0, (int)ms.Length));

                ms.Position = 0;

                var clone = model.Deserialize(ms, null, typeof(T));

                Assert.IsNotNull(clone, "clone");
                Assert.AreNotSame(obj, clone);

                return clone as T;
            }
        }

        public static void Visit<TContract, TMember, TInclude>(this TypeInfo mappedType)
            where TContract : Attribute
            where TMember : Attribute
            where TInclude : Attribute
        {
            var declaredProperties = mappedType.DeclaredProperties;

            var propertyInfos = declaredProperties as IList<PropertyInfo> ?? declaredProperties.ToList();

            Console.WriteLine(new string(Convert.ToChar("*"), 50));
            Console.WriteLine("Type={0}", mappedType.Name);
            Console.WriteLine(new string(Convert.ToChar("*"), 50));
            Console.WriteLine();
            Console.WriteLine("Properties={0}", propertyInfos.Count);
            Console.WriteLine(new string(Convert.ToChar("*"), 20));

            foreach (var prop in propertyInfos)
            {
                Console.WriteLine("Property={0}", prop.Name);
            }

            Console.WriteLine();
            Console.WriteLine("Attributes");
            Console.WriteLine(new string(Convert.ToChar("*"), 20));

            foreach (var attr in mappedType.GetCustomAttributes<TContract>(true))
            {
                Console.WriteLine(attr);
                Console.WriteLine(attr.Properties());
            }

            foreach (var attr in mappedType.GetCustomAttributes<TInclude>(true))
            {
                Console.WriteLine(attr);
                Console.WriteLine(attr.Properties());
            }

            foreach (var info in propertyInfos)
            {
                foreach (var attr in info.GetCustomAttributes<TMember>(true))
                {
                    Console.WriteLine(attr);
                    Console.WriteLine(attr.Properties());
                }
            }

            Console.WriteLine();
        }

        public static void PrintSchemas(this IEnumerable<MetaType> types, Configuration.Configuration config)
        {
            types.Each(t => Console.WriteLine(config.RuntimeTypeModel.GetSchema(t.Type)));
        }

    }
}
