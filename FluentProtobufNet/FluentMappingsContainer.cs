using System;
using System.Collections.Generic;
using System.Reflection;
using FluentProtobufNet.Helpers;
using FluentProtobufNet.Mapping;

namespace FluentProtobufNet
{
    public class FluentMappingsContainer
    {
        private readonly IList<Assembly> _assemblies = new List<Assembly>();
        private readonly List<Type> _types = new List<Type>();

        public bool WasUsed { get; set; }

        /// <summary>
        ///     Add all fluent mappings in the assembly that contains T.
        /// </summary>
        /// <typeparam name="T">Type from the assembly</typeparam>
        /// <returns>Fluent mappings configuration</returns>
        public FluentMappingsContainer AddFromAssemblyOf<T>()
        {
            return this.AddFromAssembly(typeof (T).Assembly);
        }

        /// <summary>
        ///     Add all fluent mappings in the assembly
        /// </summary>
        /// <param name="assembly">Assembly to add mappings from</param>
        /// <returns>Fluent mappings configuration</returns>
        public FluentMappingsContainer AddFromAssembly(Assembly assembly)
        {
            this._assemblies.Add(assembly);
            this.WasUsed = true;
            return this;
        }

        /// <summary>
        ///     Adds a single <see cref="IMappingProvider" /> represented by the specified type.
        /// </summary>
        /// <returns>Fluent mappings configuration</returns>
        public FluentMappingsContainer Add<T>()
        {
            return this.Add(typeof (T));
        }

        /// <summary>
        ///     Adds a single <see cref="IMappingProvider" /> represented by the specified type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>Fluent mappings configuration</returns>
        public FluentMappingsContainer Add(Type type)
        {
            if (type == null)
                throw new ArgumentNullException("type");

            this._types.Add(type);

            this.WasUsed = true;

            return this;
        }

        internal void Apply(PersistenceModel model)
        {
            foreach (var assembly in this._assemblies)
            {
                model.AddMappingsFromAssembly(assembly);
            }

            foreach (var type in this._types)
            {
                model.Add(type);
            }
        }

        public FluentMappingsContainer AddWcfMapFor(Type mapType)
        {
            var baseType = mapType.BaseType;

            if (baseType.IsDataContract())
            {
                Console.WriteLine("adding WcfSubClassMap for {0}", mapType);

                this._types.Add(typeof(WcfSubClassMap<>).MakeGenericType(mapType));
            }
            else
            {
                Console.WriteLine("adding WcfClassMap for {0}", mapType);

                this._types.Add(typeof(WcfClassMap<>).MakeGenericType(mapType));
            }         

            this.WasUsed = true;

            return this;
        }
    }
}