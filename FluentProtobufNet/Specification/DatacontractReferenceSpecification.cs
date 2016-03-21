using System.Linq;
using System.Runtime.Serialization;
using FluentProtobufNet.Helpers;

namespace FluentProtobufNet.Specification
{
    using System;
    using System.Reflection;

    public class DatacontractReferenceSpecification<TMessage> : ISpecification<PropertyInfo>
        where TMessage : class
    {
        public DatacontractReferenceSpecification()
        {
            this.DynamicType = typeof(TMessage).FullName;
        }

        public string DynamicType { get; set; }

        public bool IsSatisfied(PropertyInfo type)
        {
            var isDataContract = type.PropertyType.IsDataContract();

            if (!isDataContract) return false;

            var dc = type.PropertyType.GetCustomAttributes<DataContractAttribute>().FirstOrDefault();

            if (dc == null || !dc.IsReference) return false;

            Console.WriteLine("satisfied that [" + type.PropertyType.Name + "] on member [" + this.DynamicType + "] is a Reference Type");

            return true;
        }
    }
}