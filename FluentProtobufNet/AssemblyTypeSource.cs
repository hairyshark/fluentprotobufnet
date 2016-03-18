using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace FluentProtobufNet
{
    public class AssemblyTypeSource : ITypeSource
    {
        protected readonly Assembly Source;

        public AssemblyTypeSource(Assembly source)
        {
            if (source == null) throw new ArgumentNullException("source");

            this.Source = source;
        }

        public override int GetHashCode()
        {
            return this.Source.GetHashCode();
        }

        #region ITypeSource Members

        public virtual IEnumerable<Type> GetTypes()
        {
            return this.Source.GetTypes().OrderBy(x => x.FullName);
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