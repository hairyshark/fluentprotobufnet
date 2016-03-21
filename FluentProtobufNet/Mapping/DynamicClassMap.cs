namespace FluentProtobufNet.Mapping
{
    using FluentProtobufNet.Helpers;
    using FluentProtobufNet.Specification;
    using FluentProtobufNet.Tests;

    public class DynamicClassMap<T> : ClassMap<T>
        where T : class
    {
        public DynamicClassMap()
        {
            this.BuildUp<DynamicClassMap<T>, T, ReferenceSpecification<T>>(Indexor.GetIndex);
        }
    }
}