namespace FluentProtobufNet.Specification
{
    using System;

    using FluentProtobufNet.Helpers;

    public class AnyDataContractSpecification : ISpecification<Type>
    {
        public bool IsSatisfied(Type type)
        {
            var isSatisfied = type.IsDataContract();

            if (isSatisfied)
            {
                Console.WriteLine("satisfied with " + type.Name);
            }

            return isSatisfied;
        }
    }
}