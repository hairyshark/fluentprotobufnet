using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FluentProtobufNet.Helpers;
using FluentProtobufNet.Mapping;
using ProtoBuf.Meta;

namespace FluentProtobufNet
{
    public class PersistenceModel
    {
        private readonly RuntimeTypeModel _protobufModel;

        private readonly IList<IMappingProvider> _subclassProviders = new List<IMappingProvider>();
        protected readonly IList<IMappingProvider> ClassProviders = new List<IMappingProvider>();

        private readonly Func<object, int> _indexor;

        protected IDiagnosticLogger Log = new NullDiagnosticsLogger();

        public PersistenceModel(RuntimeTypeModel protobufModel, Func<object, int> indexor)
        {
            this._protobufModel = protobufModel;
            this._indexor = indexor;
        }

        public void AddClassMap(IMappingProvider provider)
        {
            this.ClassProviders.Add(provider);
        }

        public void AddSubclassMap(IMappingProvider provider)
        {
            this._subclassProviders.Add(provider);
        }

        public void RegisterModelType(Type type)
        {
            var mapping = type.Instantiate(this._indexor);

            var provider = mapping as IMappingProvider;

            if (provider != null)
            {
                var providerType = provider.GetType();

                if (providerType.IsMappingOf<IMapSubClasses>())
                {
                    this.Log.FluentMappingDiscovered(type);

                    this.AddSubclassMap(provider);
                }
                else if (providerType.IsMappingOf<IMapBaseClasses>())
                {
                    this.Log.FluentMappingDiscovered(type);

                    this.AddClassMap(provider);
                }
                else
                {
                    throw new InvalidOperationException("cant find any marker interface to register type source");
                }
            }
            else
            {
                throw new InvalidOperationException("Unsupported mapping type '" + type.FullName + "'");
            }
        }

        public void RegisterModelTypesFromAssemblySource(Tuple<Assembly, Type> tuple)
        {
            var constructorInfo = tuple.Item2.GetConstructor(new[] { typeof(Assembly) });

            if (constructorInfo == null)
            {
                throw new InvalidOperationException("missing constructor public " + tuple.Item2.Name + "(Assembly assembly) on type " + tuple.Item2);
            }

            var typeSourceInstance = constructorInfo.Invoke(new object[] { tuple.Item1 });

            this.AddMappingsFromSource((ITypeSource)typeSourceInstance);
        }

        public void AddMappingsFromSource(ITypeSource source)
        {
            source.GetTypeSources().Where(t => t.IsMappingOf<IMappingProvider>()).Each(this.RegisterModelType);

            this.Log.LoadedFluentMappingsFromSource(source);
        }

        public virtual void Configure(Configuration cfg)
        {
            foreach (var classMap in this.ClassProviders)
            {
                classMap.GetRuntimeTypeModel(this._protobufModel);
            }

            var subclassProvidersCopy = this._subclassProviders.ToList();

            IMappingProvider subclassMap;

            while ((subclassMap = subclassProvidersCopy.FirstOrDefault(sc => sc.CanBeResolvedUsing(this._protobufModel))) != null)
            {
                subclassMap.GetRuntimeTypeModel(this._protobufModel);
                subclassProvidersCopy.Remove(subclassMap);
            }

            if (subclassProvidersCopy.Any())
            {
                throw new Exception("Couldn't resolve all subclasses. We can't leave floaters.");
            }

            cfg.RuntimeTypeModel = this._protobufModel;
        }
    }
}