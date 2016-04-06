namespace FluentProtobufNet.Specification
{
    using System;
    using System.CodeDom;
    using System.Collections.Generic;

    public class IsReferenceSpecification<T> : ISpecification<Type>
        where T : class
    {
        private static readonly IList<Type> Cache = new List<Type>();

        public IsReferenceSpecification()
        {
            this.FullName = typeof(T).FullName;
        }

        public string FullName { get; set; }

        public bool IsSatisfied(Type type)
        {
            if (Cache.Contains(type))
            {
                return true;
            }

            if (typeof(IEnumerable<>).IsAssignableFrom(type))
            {
                return false;
            }

            if (type.IsSubclassOf(typeof(T)))
            {
                Add(type);
            }

            if (type == typeof(T))
            {
                Add(type);
                return true;
            }

            return false;
        }

        private static void Add(Type type)
        {
            Console.WriteLine(@"Adding {0} to found Reference descriptors", type);
            Cache.Add(type);
        }
    }
}
