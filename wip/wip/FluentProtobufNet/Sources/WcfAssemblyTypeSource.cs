using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FluentProtobufNet.Helpers;
using FluentProtobufNet.Mapping;
using FluentProtobufNet.Specification;

namespace FluentProtobufNet.Sources
{
    using System.Runtime.Serialization;

    public class WcfAssemblyTypeSource<TSpecification> : AssemblyTypeSource<TSpecification>
        where TSpecification : ISpecification<Type>, new()
    {
        public WcfAssemblyTypeSource(Assembly source) : base(source)
        {
        }

        #region ITypeSource Members

        protected override IEnumerable<Type> GetPossibleTypes()
        {
            var possibleTypes = base.GetPossibleTypes().Where(t => t.IsDataContract()).ToList();

            var clone = new Type[possibleTypes.Count];

            possibleTypes.CopyTo(clone);

            // now look at KnownTypes of each DataContract and Add Them (NOT ALL KNOWNTYPES ARE ATTRIBUTED)
            foreach (var knownTypeAttribute in clone.Select(type => type.GetCustomAttributes<KnownTypeAttribute>().ToList()).SelectMany(attrs => attrs.Where(a => !possibleTypes.Contains(a.Type))))
            {
                possibleTypes.Add(knownTypeAttribute.Type);
            }

            return possibleTypes;
        }

        protected override bool IsDerived(Type type)
        {
            var baseIsDataContract = type.BaseType.IsDataContract();

            return baseIsDataContract;
        }

        public override Type ClassMapType
        {
            get { return typeof(WcfClassMap<>); }
        }

        public override Type SubClassMapType
        {
            get { return typeof(WcfSubClassMap<>); }
        }

        #endregion
    }
}