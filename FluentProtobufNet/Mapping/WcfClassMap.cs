namespace FluentProtobufNet.Mapping
{
    using System.Runtime.Serialization;

    using FluentProtobufNet.Helpers;
    using FluentProtobufNet.Specification;
    using FluentProtobufNet.Tests;

    public class WcfClassMap<T> : ClassMap<T>
        where T : class
    {
        public WcfClassMap()
        {
            this.BuildUp<WcfClassMap<T>, T, ReferenceSpecification<T>>(Indexor.GetIndex, Extensions.HasAttribute<DataMemberAttribute>);
        }
    }
}