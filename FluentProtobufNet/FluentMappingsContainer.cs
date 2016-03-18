using System;
using System.Collections.Generic;
using System.Reflection;

namespace FluentProtobufNet
{
    public class FluentMappingsContainer
    {
        private readonly IList<Tuple<Assembly, Type>> _assemblySources = new List<Tuple<Assembly, Type>>();

        private readonly List<Type> _types = new List<Type>();

        /// <summary>
        ///     Add all fluent mappings in the assembly that contains T.
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TType"></typeparam>
        /// <returns>Fluent mappings configuration</returns>
        public FluentMappingsContainer AddFromAssemblySource<TType, TSource>()
            where TType : class
            where TSource : ITypeSource
        {
            return this.AddFromAssemblySource(typeof(TType).Assembly, typeof(TSource));
        }

        /// <summary>
        ///     Add all fluent mappings in the assembly
        /// </summary>
        /// <param name="assembly">Assembly to add mappings from</param>
        /// <param name="typeSource"></param>
        /// <returns>Fluent mappings configuration</returns>
        private FluentMappingsContainer AddFromAssemblySource(Assembly assembly, Type typeSource)
        {
            this._assemblySources.Add(new Tuple<Assembly, Type>(assembly, typeSource));
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

            return this;
        }

        internal void Apply(PersistenceModel model)
        {
            foreach (var tuple in this._assemblySources)
            {
                model.AddMappingsFromAssemblySource(tuple);
            }

            foreach (var type in this._types)
            {
                model.Add(type);
            }
        }
    }
}