using System;

namespace FluentProtobufNet
{
    public class NameSpaceSpecification<TClass> : ISpecification
    {
        public bool IsSatisfied(Type type)
        {
            var isSatisfied = type.Namespace != null && type.Namespace.Equals(typeof (TClass).Namespace);

            if (isSatisfied)
            {
                Console.WriteLine("satisfied with " + type.Name);
            }

            return isSatisfied;
        }
    }
}