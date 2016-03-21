using System.Runtime.Serialization;
using FluentProtobufNet.Helpers;
using FluentProtobufNet.Specification;

namespace FluentProtobufNet.Mapping
{
    public class WcfSubClassMap<T> : SubclassMap<T>
        where T : class
    {
        public WcfSubClassMap(int discriminator) : base(discriminator)
        {
            this.BuildUp<WcfSubClassMap<T>, T, DatacontractReferenceSpecification<T>>(SeededIndexor.GetIndex,
                Extensions.HasAttribute<DataMemberAttribute>);
        }
    }
}