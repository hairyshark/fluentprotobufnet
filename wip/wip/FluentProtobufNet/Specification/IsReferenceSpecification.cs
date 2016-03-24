namespace FluentProtobufNet.Specification
{
    using System;

    public class IsReferenceSpecification<TMessage> : ISpecification<Type>
        where TMessage : class
    {
        public string DynamicType { get; set; }

        public IsReferenceSpecification()
        {
            this.DynamicType = typeof(TMessage).FullName;
        }

        public bool IsSatisfied(Type type)
        {
            var test1 = typeof(TMessage).FullName == type.FullName;

            var isSatisfied = test1;

            if (!isSatisfied)
            {
                return false;
            }

            Console.WriteLine("satisfied that [" + type.FullName + "] on member [" + this.DynamicType + "] is a Reference Type");

            return true;
        }
    }
}