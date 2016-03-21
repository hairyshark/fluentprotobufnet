using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FluentProtobufNet.Helpers;
using FluentProtobufNet.Mapping;
using FluentProtobufNet.Specification;

namespace FluentProtobufNet.Sources
{
    public class WcfAssemblyTypeSource<TSpecification> : AssemblyTypeSource<TSpecification>
        where TSpecification : ISpecification<Type>, new()
    {
        public WcfAssemblyTypeSource(Assembly source) : base(source)
        {
        }

        #region ITypeSource Members

        protected override IEnumerable<Type> GetPossibleTypes()
        {
            return base.GetPossibleTypes().Where(t => t.IsDataContract());
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