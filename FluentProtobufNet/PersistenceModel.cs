namespace FluentProtobufNet
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using FluentProtobufNet.Helpers;
    using FluentProtobufNet.Mapping;

    using ProtoBuf.Meta;

    public class PersistenceModel
    {
        protected readonly IList<IMappingProvider> ClassProviders = new List<IMappingProvider>();

        protected IDiagnosticLogger Log = new NullDiagnosticsLogger();

        private readonly RuntimeTypeModel _protobufModel;

        private readonly Func<object, int> indexor;

        private readonly IList<IMappingProvider> _subclassProviders = new List<IMappingProvider>();

        public PersistenceModel(RuntimeTypeModel protobufModel, Func<object, int> indexor)
        {
            this._protobufModel = protobufModel;
            this.indexor = indexor;
        }

        public void Add(IMappingProvider provider)
        {
            this.ClassProviders.Add(provider);
        }

        public void Add(Type type)
        {
            var mapping = type.Instantiate(this.indexor);

            if (mapping is IMappingProvider)
            {
                if (mapping.GetType().BaseType != null && mapping.GetType().BaseType.IsGenericType)
                {
                    if (mapping.GetType().BaseType.GetGenericTypeDefinition() == typeof(ClassMap<>))
                    {
                        Log.FluentMappingDiscovered(type);
                        Add((IMappingProvider)mapping);
                    }
                    else if (mapping.GetType().BaseType.GetGenericTypeDefinition() == typeof(SubclassMap<>))
                    {
                        AddSubclassMap((IMappingProvider)mapping);
                    }
                }
            }
            else
                throw new InvalidOperationException("Unsupported mapping type '" + type.FullName + "'");
        }

        public void AddMappingsFromAssembly(Assembly assembly)
        {
            this.AddMappingsFromSource(new AssemblyTypeSource(assembly));
        }

        public void AddMappingsFromSource(ITypeSource source)
        {
            source.GetTypes().Where(IsMappingOf<IMappingProvider>).Each(this.Add);

            this.Log.LoadedFluentMappingsFromSource(source);
        }

        public void AddSubclassMap(IMappingProvider provider)
        {
            this._subclassProviders.Add(provider);
        }

        public virtual void Configure(Configuration cfg)
        {
            foreach (var classMap in this.ClassProviders)
            {
                classMap.GetRuntimeTypeModel(this._protobufModel);
            }

            var subclassProvidersCopy = this._subclassProviders.ToList();
            IMappingProvider subclassMap;
            while (
                (subclassMap = subclassProvidersCopy.FirstOrDefault(sc => sc.CanBeResolvedUsing(this._protobufModel)))
                != null)
            {
                subclassMap.GetRuntimeTypeModel(this._protobufModel);
                subclassProvidersCopy.Remove(subclassMap);
            }

            if (subclassProvidersCopy.Any())
            {
                throw new Exception("Couldn't resolve all subclasses");
            }

            cfg.RuntimeTypeModel = this._protobufModel;
        }

        private static bool IsMappingOf<T>(Type type)
        {
            return !type.IsGenericType && typeof(T).IsAssignableFrom(type);
        }
    }
}