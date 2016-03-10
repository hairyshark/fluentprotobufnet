using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using FluentProtobufNet.Mapping;

namespace FluentProtobufNet.Tests
{
    public static class DynamicTypeMapper
    {
        public static void DynamicSubclassMap<TMapper, TMessage>(this TMapper mapper, int subclassField) where TMapper : SubclassMap<TMessage>
        {
            mapper.SubclassFieldId(subclassField);

            DynamicClassMap<TMapper, TMessage>(mapper);
        }

        public static void DynamicClassMap<TMapper, TMessage>(this TMapper mapper) where TMapper : ClassMap<TMessage>
        {
            // find every public invokable instance targetMember
            const BindingFlags flags = BindingFlags.Public | BindingFlags.GetProperty | BindingFlags.Instance;

            var seed = 1;

            foreach (var result in typeof(TMessage).GetProperties(flags).Select(Build<TMessage>))
            {
                if (result.Item2)
                {
                    mapper.References(result.Item1, seed++);
                }
                else
                {
                    mapper.Map(result.Item1, seed++);
                }
            }
        }

        private static Tuple<Expression<Func<TMessage, object>>, bool> Build<TMessage>(PropertyInfo targetMember)
        {
            ParameterExpression typeParam = Expression.Parameter(typeof(TMessage), "m");

            MemberExpression memberExpression = Expression.Property(typeParam, targetMember.Name);

            UnaryExpression castedToObject = Expression.Convert(memberExpression, typeof(object));

            //var expressionCall = Expression.Property(memberExpression, castedToObject);

            var expression = Expression.Lambda<Func<TMessage, object>>(castedToObject, typeParam);

            var isReference = targetMember.PropertyType == typeof(TMessage);

            return new Tuple<Expression<Func<TMessage, object>>, bool>(expression, isReference);
        }
    }
}