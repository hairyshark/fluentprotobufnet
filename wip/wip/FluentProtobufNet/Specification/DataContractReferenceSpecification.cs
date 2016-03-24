namespace FluentProtobufNet.Specification
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.Serialization;

    using FluentProtobufNet.Helpers;

    public class DataContractReferenceSpecification : ISpecification<Type>
    {

        private static readonly IList<Type> Cache = new List<Type>(); 

        public bool IsSatisfied(Type type)
        {
            if (Cache.Contains(type))
            {
                return true;
            }

            if (!type.IsDataContract())
            {
                return false;
            }

            if (Find(type))
            {
                Add(type);
                return true;
            }

            // check base types
            var baseType = type.BaseType;

            while (baseType != null && baseType.BaseType != null)
            {
                if (Find(baseType))
                {
                    Add(baseType);
                    return true;
                }

                baseType = baseType.BaseType;
            }

            return false;
        }

        private static void Add(Type type)
        {
            Console.WriteLine("Adding {0} to found Reference Types", type);
            Cache.Add(type);
        }

        private static bool Find(MemberInfo type)
        {
            var attrs = type.GetCustomAttributes<DataContractAttribute>(true);

            return attrs.Any(CheckIt);
        }

        private static bool CheckIt(DataContractAttribute dataContractAttribute)
        {
            return dataContractAttribute.IsReference;
        }
    }
}