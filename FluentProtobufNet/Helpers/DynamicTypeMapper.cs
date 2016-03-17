using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using FluentProtobufNet.Mapping;

namespace FluentProtobufNet.Helpers
{
    using System.Collections.Generic;

    public static class DynamicTypeMapper
    {
        public static void DynamicSubclassMap<TMapper, TMessage>(this TMapper mapper, int subclassField, Func<PropertyInfo, int> indexor, Func<PropertyInfo, bool> selector = null) where TMapper : SubclassMap<TMessage>
        {
            mapper.SubclassFieldId(subclassField);

            DynamicClassMap<TMapper, TMessage>(mapper, indexor, selector);
        }

        public static void DynamicClassMap<TMapper, TMessage>(this TMapper mapper, Func<PropertyInfo, int> indexor, Func<PropertyInfo, bool> selector = null) where TMapper : ClassMap<TMessage>
        {
            // find every public invokable instance targetMember
            const BindingFlags flags = BindingFlags.Public | BindingFlags.GetProperty | BindingFlags.Instance;

            IEnumerable<PropertyInfo> propertyInfos = typeof(TMessage).GetProperties(flags);

            if (selector != null)
            {
                propertyInfos = propertyInfos.Where(selector);
            }

            foreach (var buildResult in propertyInfos.Select(p => Build<TMessage>(p, indexor)))
            {
                if (buildResult.Item2)
                {
                    mapper.References(buildResult.Item1, buildResult.Item3);
                }
                else
                {
                    mapper.Map(buildResult.Item1, buildResult.Item3);
                }
            }
        }

        private static Tuple<Expression<Func<TMessage, object>>, bool, int> Build<TMessage>(PropertyInfo targetMember, Func<PropertyInfo, int> indexor)
        {
            ParameterExpression typeParam = Expression.Parameter(typeof(TMessage), "m");

            MemberExpression memberExpression = Expression.Property(typeParam, targetMember.Name);

            UnaryExpression castedToObject = Expression.Convert(memberExpression, typeof(object));

            var expression = Expression.Lambda<Func<TMessage, object>>(castedToObject, typeParam);

            var isReference = targetMember.PropertyType == typeof(TMessage);

            return new Tuple<Expression<Func<TMessage, object>>, bool, int>(expression, isReference, indexor(targetMember));
        }
    }
}