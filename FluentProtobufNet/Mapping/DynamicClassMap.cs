using FluentProtobufNet.Helpers;
using FluentProtobufNet.Specification;

namespace FluentProtobufNet.Mapping
{
    public class DynamicClassMap<T> : ClassMap<T>
        where T : class
    {
        public DynamicClassMap()
        {
            this.BuildUp<DynamicClassMap<T>, T, DatacontractReferenceSpecification<T>>(SeededIndexor.GetIndex);
        }
    }
}