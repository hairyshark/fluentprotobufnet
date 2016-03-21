using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using FluentProtobufNet.Mapping;

namespace FluentProtobufNet.Helpers
{
    using System.Collections.Generic;

    using Specification;

    public static class DynamicTypeMapper
    {
        public static void BuildUp<TMapper, TMessage, TReferenceSpecification>(this TMapper mapper, Func<PropertyInfo, int> indexor, Func<PropertyInfo, bool> selector = null) 
            where TMapper : ClassMap<TMessage>
            where TReferenceSpecification : ISpecification<PropertyInfo>, new()
        {
            // find every public invokable instance targetMember
            const BindingFlags flags = BindingFlags.Public | BindingFlags.GetProperty | BindingFlags.Instance;

            IEnumerable<PropertyInfo> propertyInfos = typeof(TMessage).GetProperties(flags);

            if (selector != null)
            {
                propertyInfos = propertyInfos.Where(selector);
            }

            foreach (var buildResult in propertyInfos.Select(p => Build<TMessage>(p, indexor, new TReferenceSpecification())))
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

        private static Tuple<Expression<Func<TMessage, object>>, bool, int> Build<TMessage>(PropertyInfo targetMember, Func<PropertyInfo, int> indexor, ISpecification<PropertyInfo> referenceSpecification)
        {
            ParameterExpression typeParam = Expression.Parameter(typeof(TMessage), "m");

            MemberExpression memberExpression = Expression.Property(typeParam, targetMember.Name);

            UnaryExpression castedToObject = Expression.Convert(memberExpression, typeof(object));

            var expression = Expression.Lambda<Func<TMessage, object>>(castedToObject, typeParam);

            var isReference = referenceSpecification.IsSatisfied(targetMember);

            return new Tuple<Expression<Func<TMessage, object>>, bool, int>(expression, isReference, indexor(targetMember));
        }
    }
}