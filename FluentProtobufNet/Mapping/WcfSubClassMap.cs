using System.Runtime.Serialization;
using FluentProtobufNet.Helpers;
using FluentProtobufNet.Specification;
using FluentProtobufNet.Tests;

namespace FluentProtobufNet.Mapping
{
    public class WcfSubClassMap<T> : SubclassMap<T>
        where T : class
    {
        public WcfSubClassMap(int discriminator) : base(discriminator)
        {
            this.BuildUp<WcfSubClassMap<T>, T, ReferenceSpecification<T>>(Indexor.GetIndex,
                Extensions.HasAttribute<DataMemberAttribute>);
        }
    }
}