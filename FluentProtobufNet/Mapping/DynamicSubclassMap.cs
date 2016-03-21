namespace FluentProtobufNet.Mapping
{
    using FluentProtobufNet.Helpers;
    using FluentProtobufNet.Specification;
    using FluentProtobufNet.Tests;

    public class DynamicSubclassMap<T> : SubclassMap<T>
        where T : class
    {
        public DynamicSubclassMap(int fieldNumber) : base(fieldNumber)
        {
            this.BuildUp<DynamicSubclassMap<T>, T, ReferenceSpecification<T>>(Indexor.GetIndex);            
        }    
    }
}