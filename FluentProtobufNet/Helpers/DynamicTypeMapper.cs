using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using FluentProtobufNet.Mapping;

namespace FluentProtobufNet.Helpers
{
    public static class DynamicTypeMapper
    {
        public static void DynamicSubclassMap<TMapper, TMessage>(this TMapper mapper, int subclassField, Func<PropertyInfo, int> indexor) where TMapper : SubclassMap<TMessage>
        {
            mapper.SubclassFieldId(subclassField);

            DynamicClassMap<TMapper, TMessage>(mapper, indexor);
        }

        public static void DynamicClassMap<TMapper, TMessage>(this TMapper mapper, Func<PropertyInfo, int> indexor) where TMapper : ClassMap<TMessage>
        {
            // find every public invokable instance targetMember
            const BindingFlags flags = BindingFlags.Public | BindingFlags.GetProperty | BindingFlags.Instance;

            foreach (var result in typeof(TMessage).GetProperties(flags).Select(p => Build<TMessage>(p, indexor)))
            {
                if (result.Item2)
                {
                    mapper.References(result.Item1, result.Item3);
                }
                else
                {
                    mapper.Map(result.Item1, result.Item3);
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