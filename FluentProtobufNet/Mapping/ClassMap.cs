using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using FluentProtobufNet.Helpers;
using ProtoBuf.Meta;

namespace FluentProtobufNet.Mapping
{
    public abstract class ClassMap<T> : IMappingProvider, IMapBaseClasses
    {
        protected ClassMap()
        {
            this.Fields = new List<PropertyMapping>();
        }

        public IList<PropertyMapping> Fields { get; set; }


        internal Type EntityType
        {
            get { return typeof (T); }
        }

        public virtual RuntimeTypeModel GetRuntimeTypeModel(RuntimeTypeModel protobufModel)
        {
            var protoType = protobufModel.Add(typeof (T), false);
            foreach (var f in this.Fields)
            {
                protoType.Add(f.FieldNumber, f.Member.Name);
                protoType.GetFields().Single(newField => newField.FieldNumber == f.FieldNumber).AsReference =
                    f.AsReference;
            }

            return protobufModel;
        }

        public virtual bool CanBeResolvedUsing(RuntimeTypeModel protobufModel)
        {
            return true;
        }

        public PropertyMapping Map(Expression<Func<T, object>> memberExpression, int fieldNumber)
        {
            return this.Map(memberExpression.ToMember(), fieldNumber);
        }

        private PropertyMapping Map(Member member, int fieldNumber)
        {
            //­OnMemberMapped(member);

            var field = new PropertyMapping
            {
                Member = member,
                FieldNumber = fieldNumber,
                AsReference = false,
                Type = typeof(T)
            };
            this.Fields.Add(field);

            return field;
        }


        public PropertyMapping References<TOther>(Expression<Func<T, TOther>> memberExpression, int fieldNumber)
        {
            return this.References(memberExpression.ToMember(), fieldNumber);
        }

        public PropertyMapping References(Expression<Func<T, object>> memberExpression, int fieldNumber)
        {
            return this.References(memberExpression.ToMember(), fieldNumber);
        }

        private PropertyMapping References(Member member, int fieldNumber)
        {
            //OnMemberMapped(member);

            var field = new PropertyMapping
            {
                Member = member,
                FieldNumber = fieldNumber,
                AsReference = true,
                Type = typeof(T)
            };
            this.Fields.Add(field);

            return field;
        }

        protected bool AllPublic(PropertyInfo arg)
        {
            return true;
        }
    }

    public class PropertyMapping
    {
        public Member Member { get; set; }
        public int FieldNumber { get; set; }
        public bool AsReference { get; set; }
        public Type Type { get; set; }
    }
}