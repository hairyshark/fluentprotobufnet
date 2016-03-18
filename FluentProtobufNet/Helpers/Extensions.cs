using System.Runtime.Serialization;

namespace FluentProtobufNet.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Exceptions;

    public static class Extensions
    {
        public static void Each<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach (var item in source)
                action(item);
        }

        public static object Instantiate(this Type type, Func<object, int> indexor)
        {
            var constructor = ReflectHelper.GetClassMapConstructor(type);
            var constructor2 = ReflectHelper.GetSubclassMapConstructor(type);

            if (constructor == null && constructor2 == null)
            {
                throw new MissingConstructorException(type);
            }

            return constructor != null ? constructor.Invoke(null) : constructor2.Invoke(new object[] { indexor(type) });
        }

        public static bool HasAttribute<TAttribute>(this MemberInfo arg)
            where TAttribute : Attribute
        {
            return arg.GetCustomAttributes<TAttribute>(false).FirstOrDefault() != null;
        }

        public static bool IsObject(this Type type)
        {
            return type.BaseType == null && type.FullName.Equals(typeof(object).FullName);
        }

        public static bool IsDataContract(this Type type)
        {
            return type.HasAttribute<DataContractAttribute>();
        }
    }
}