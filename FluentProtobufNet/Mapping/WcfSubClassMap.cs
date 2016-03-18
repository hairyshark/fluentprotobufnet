namespace FluentProtobufNet.Mapping
{
    using System.Runtime.Serialization;

    using Helpers;
    using Tests;

    public class WcfSubClassMap<T> : SubclassMap<T>, IMapSubClasses
    {
        public WcfSubClassMap(int discriminator) : base(discriminator)
        {
            this.DynamicClassMap<WcfSubClassMap<T>, T>(Indexor.GetIndex, Extensions.HasAttribute<DataMemberAttribute>);
        }
    }
}
