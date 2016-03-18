using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FluentProtobufNet.Helpers;
using FluentProtobufNet.Mapping;

namespace FluentProtobufNet
{
    public class WcfAssemblyTypeSource : AssemblyTypeSource
    {
        public WcfAssemblyTypeSource(Assembly source) : base(source)
        {
        }

        #region ITypeSource Members

        public override IEnumerable<Type> GetTypes()
        {
            var allDataContracts = base.GetTypes().Where(t => t.IsDataContract());

            var types = new List<Type>();

            foreach (var type in allDataContracts)
            {
                var baseType = type.BaseType;

                if (baseType.IsDataContract())
                {
                    Console.WriteLine("adding WcfSubClassMap for {0}", type);

                    types.Add(typeof(WcfSubClassMap<>).MakeGenericType(type));

                }
                else
                {
                    Console.WriteLine("adding WcfClassMap for {0}", type);

                    types.Add(typeof(WcfClassMap<>).MakeGenericType(type));
                }
            }

            return types;
        }

        public void LogSource(IDiagnosticLogger logger)
        {
            if (logger == null) throw new ArgumentNullException("logger");

            logger.LoadedFluentMappingsFromSource(this);
        }

        public string GetIdentifier()
        {
            return this.Source.GetName().FullName;
        }

        #endregion
    }
}