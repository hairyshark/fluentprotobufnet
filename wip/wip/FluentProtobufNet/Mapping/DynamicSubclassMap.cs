using FluentProtobufNet.Helpers;
using FluentProtobufNet.Specification;

namespace FluentProtobufNet.Mapping
{
    public class DynamicSubclassMap<T> : SubclassMap<T>
        where T : class
    {
        public DynamicSubclassMap(int fieldNumber) : base(fieldNumber)
        {
            this.BuildUp<DynamicSubclassMap<T>, T, IsReferenceSpecification<T>>(SeededIndexor.GetIndex);
        }
    }
}