namespace FluentProtobufNet.Specification
{
    using System;
    using System.Reflection;

    public class ReferenceSpecification<TMessage> : ISpecification<PropertyInfo>
        where TMessage : class
    {
        public ReferenceSpecification()
        {
            this.DynamicType = typeof(TMessage);
        }

        public Type DynamicType { get; set; }

        public bool IsSatisfied(PropertyInfo type)
        {
            var isSatisfied = type.Name.Equals(this.DynamicType.Name);

            if (isSatisfied)
            {
                Console.WriteLine("satisfied with " + type.Name + "on message " + this.DynamicType);
            }

            return isSatisfied;
        }
    }
}