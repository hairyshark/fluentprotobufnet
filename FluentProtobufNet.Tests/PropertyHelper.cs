using System;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using FluentProtobufNet.Helpers;

namespace FluentProtobufNet.Tests
{
    public static class PropertyHelper
    {
        public static string Properties(this object item)
        {
            var sb = new StringBuilder();

            foreach (var propertyInfo in item.GetType().GetProperties())
            {
                sb.Append(string.Format("{0}={1};", propertyInfo.Name, propertyInfo.GetMethod.Invoke(item, null)));
            }

            return sb.ToString();
        }
    }
}