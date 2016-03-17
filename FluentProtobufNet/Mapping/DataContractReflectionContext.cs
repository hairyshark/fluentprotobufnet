namespace FluentProtobufNet.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Reflection.Context;
    using System.Runtime.Serialization;

    using ProtoBuf;

    public class DataContractReflectionContext : CustomReflectionContext
    {
        protected override IEnumerable<object> GetCustomAttributes(MemberInfo member,
                                                                   IEnumerable<object> declaredAttributes)
        {
            var dc = FirstOrDefault<DataContractAttribute>(member);
            var dm = FirstOrDefault<DataMemberAttribute>(member);
            var kt = FirstOrDefault<KnownTypeAttribute>(member);

            var result = new List<object>();

            if (dc != null)
            {
                result.Add(new ProtoContractAttribute { InferTagFromName = true });
            }

            if (kt != null)
            {
                result.Add(new ProtoIncludeAttribute(member.GetIndex(), kt.Type));
            }

            if (result.Any())
            {
                return result;
            }

            if (dm == null)
            {
                return base.GetCustomAttributes(member, declaredAttributes);
            }

            result.Add(new ProtoMemberAttribute(member.GetIndex()) { Name = member.Name });

            return result;

        }

        private static TAttribute FirstOrDefault<TAttribute>(MemberInfo member) 
            where TAttribute : Attribute
        {
            return member.GetCustomAttributes<TAttribute>(false).FirstOrDefault();
        }
    }
}