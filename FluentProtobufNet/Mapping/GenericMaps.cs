namespace FluentProtobufNet.Mapping
{
    using System.Runtime.Serialization;

    using FluentProtobufNet.Helpers;
    using FluentProtobufNet.Tests;

    public class WcfClassMap<T> : ClassMap<T>
    {
        public WcfClassMap()
        {
            this.DynamicClassMap<WcfClassMap<T>, T>(Indexor.GetIndex, Extensions.HasAttribute<DataMemberAttribute>);
        }
    }

    public class WcfSubClassMap<T> : SubclassMap<T>
    {
        public WcfSubClassMap(int discriminator)
        {
            this.DynamicSubclassMap<WcfSubClassMap<T>, T>(discriminator, Indexor.GetIndex, Extensions.HasAttribute<DataMemberAttribute>);
        }
    }
}
