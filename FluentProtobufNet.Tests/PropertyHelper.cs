namespace FluentProtobufNet.Tests
{
    using System;
    using System.Reflection;
    using System.Text;

    using NUnit.Framework;

    public static class PropertyHelper
    {
        public const BindingFlags defaultFlags = BindingFlags.Public | BindingFlags.GetProperty | BindingFlags.Instance;

        public static string Properties(this object item, BindingFlags flags = defaultFlags)
        {
            var sb = new StringBuilder();

            foreach (var propertyInfo in item.GetType().GetProperties(flags))
            {
                sb.Append(string.Format("{0}={1};", propertyInfo.Name, propertyInfo.GetMethod.Invoke(item, null)));
            }

            return sb.ToString();
        }

        public static void WithPropertyCount(this ModelDescriptor descriptor, int expected, BindingFlags flags = BindingFlags.Public | BindingFlags.Instance)
        {
            Assert.AreEqual(expected, descriptor.TypeSource.GetProperties(flags).Length);
        }
    }
}
