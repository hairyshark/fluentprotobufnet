using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Context;
using System.Runtime.Serialization;
using ProtoBuf;

namespace FluentProtobufNet.Mapping
{
    public class DataContractReflectionContext : CustomReflectionContext
    {
        protected override IEnumerable<object> GetCustomAttributes(MemberInfo member,
            IEnumerable<object> declaredAttributes)
        {
            var result = new List<object>();

            if (FirstOrDefault<DataContractAttribute>(member) != null)
            {
                result.Add(new ProtoContractAttribute {InferTagFromName = true});
            }

            if (FirstOrDefault<KnownTypeAttribute>(member) != null)
            {
                result.Add(new ProtoIncludeAttribute(member.GetIndex(), FirstOrDefault<KnownTypeAttribute>(member).Type));
            }

            if (result.Any())
            {
                return result;
            }

            if (FirstOrDefault<DataMemberAttribute>(member) == null)
            {
                return base.GetCustomAttributes(member, declaredAttributes);
            }

            result.Add(new ProtoMemberAttribute(member.GetIndex()) {Name = member.Name});

            return result;
        }

        private static TAttribute FirstOrDefault<TAttribute>(MemberInfo member)
            where TAttribute : Attribute
        {
            return member.GetCustomAttributes<TAttribute>(false).FirstOrDefault();
        }
    }
}