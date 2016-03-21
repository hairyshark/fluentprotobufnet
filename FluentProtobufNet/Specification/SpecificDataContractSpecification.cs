namespace FluentProtobufNet.Specification
{
    using System;

    using FluentProtobufNet.Helpers;

    public class SpecificDataContractSpecification<TDataContract> : ISpecification<Type>
    {
        public SpecificDataContractSpecification()
        {
            this.DynamicType = typeof(TDataContract).FullName;
        }

        public string DynamicType { get; set; }

        public bool IsSatisfied(Type type)
        {
            var isSatisfied = type.IsDataContract() && type.FullName.Equals(this.DynamicType);

            if (isSatisfied)
            {
                Console.WriteLine("satisfied with " + type.FullName);
            }

            return isSatisfied;
        }
    }
}