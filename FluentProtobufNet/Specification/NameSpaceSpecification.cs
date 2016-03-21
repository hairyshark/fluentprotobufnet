namespace FluentProtobufNet.Specification
{
    using System;

    public class NameSpaceSpecification<TClass> : ISpecification<Type>
    {
        public bool IsSatisfied(Type type)
        {
            var isSatisfied = type.Namespace != null && type.Namespace.Equals(typeof (TClass).Namespace);

            if (isSatisfied)
            {
                Console.WriteLine("satisfied that [" + type.Name + "] matches namespace of TClass [" + typeof (TClass).Namespace + "]");
            }

            return isSatisfied;
        }
    }
}