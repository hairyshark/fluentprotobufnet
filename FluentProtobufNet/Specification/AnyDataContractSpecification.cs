namespace FluentProtobufNet.Specification
{
    using System;

    using Helpers;

    public class AnyDataContractSpecification : ISpecification<Type>
    {
        public bool IsSatisfied(Type type)
        {
            var isSatisfied = type.IsDataContract();

            if (isSatisfied)
            {
                Console.WriteLine("satisfied that [" + type.FullName + "] is a datacontract");
            }

            return isSatisfied;
        }
    }
}