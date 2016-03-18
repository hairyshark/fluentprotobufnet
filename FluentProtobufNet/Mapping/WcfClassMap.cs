using System.Runtime.Serialization;
using FluentProtobufNet.Helpers;
using FluentProtobufNet.Tests;

namespace FluentProtobufNet.Mapping
{
    public class WcfClassMap<T> : ClassMap<T>, IMapBaseClasses
    {
        public WcfClassMap()
        {
            this.DynamicClassMap<WcfClassMap<T>, T>(Indexor.GetIndex, Extensions.HasAttribute<DataMemberAttribute>);
        }
    }
}