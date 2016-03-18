using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FluentProtobufNet.Helpers;
using FluentProtobufNet.Mapping;

namespace FluentProtobufNet
{
    public class AssemblyTypeSource<TSpecification> : ITypeSource
        where TSpecification : ISpecification, new()
    {
        protected readonly Assembly Source;
        private readonly TSpecification specification;

        public AssemblyTypeSource(Assembly source)
        {
            if (source == null) throw new ArgumentNullException("source");

            this.Source = source;

            this.specification = new TSpecification();
        }

        public override int GetHashCode()
        {
            return this.Source.GetHashCode();
        }

        #region ITypeSource Members

        protected virtual IEnumerable<Type> GetPossibleTypes()
        {
            return this.Source.GetTypes().Where(this.specification.IsSatisfied).OrderBy(x => x.FullName);
        }

        public IEnumerable<Type> GetTypeSources()
        {
            var types = new List<Type>();

            var possibleTypes = this.GetPossibleTypes().ToList();

            foreach (var type in possibleTypes)
            {
                if (this.IsDerived(type))
                {
                    this.AddSubClassMapType(type, types);
                }
                else
                {
                    this.AddClassMapType(type, types);
                }
            }

            return types;
        }

        protected virtual bool IsDerived(Type type)
        {
            var baseTypeIsObject = type.BaseType.IsObject();

            return !baseTypeIsObject;
        }

        protected void AddSubClassMapType(Type type, List<Type> types)
        {
            Console.WriteLine("adding SubClassMapType for {0}", type);

            types.Add(this.SubClassMapType.MakeGenericType(type));
        }

        protected void AddClassMapType(Type type, List<Type> types)
        {
            Console.WriteLine("adding ClassMapType for {0}", type);

            types.Add(this.ClassMapType.MakeGenericType(type));
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

        public virtual Type ClassMapType
        {
            get { return typeof (ClassMap<>); }
        }

        public virtual Type SubClassMapType
        {
            get { return typeof(SubclassMap<>); }
        }

        #endregion
    }
}