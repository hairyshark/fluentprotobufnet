namespace FluentProtobufNet.Mapping
{
    using System.Runtime.Serialization;

    using FluentProtobufNet.Specification;

    using Helpers;
    using Tests;

    public class WcfSubClassMap<T> : SubclassMap<T>
        where T : class
    {
        public WcfSubClassMap(int discriminator) : base(discriminator)
        {
            this.BuildUp<WcfSubClassMap<T>, T, ReferenceSpecification<T>>(Indexor.GetIndex, Extensions.HasAttribute<DataMemberAttribute>);
        }
    }
}
