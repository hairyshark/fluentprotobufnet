using System.Runtime.Serialization;
using FluentProtobufNet.Helpers;
using FluentProtobufNet.Specification;

namespace FluentProtobufNet.Mapping
{
    public class WcfClassMap<T> : ClassMap<T>
        where T : class
    {
        public WcfClassMap()
        {
            this.BuildUp<WcfClassMap<T>, T, DatacontractReferenceSpecification<T>>(SeededIndexor.GetIndex,
                Extensions.HasAttribute<DataMemberAttribute>);
        }
    }
}