using System;
using System.Linq;
using FluentProtobufNet.Exceptions;
using ProtoBuf.Meta;

namespace FluentProtobufNet.Mapping
{
    public abstract class SubclassMap<T>: ClassMap<T>, IMapSubClasses
    {
        private readonly int _subclassFieldId;
        private readonly bool _fieldIdSet;

        protected SubclassMap(int fieldNumber)
        {
            this._subclassFieldId = fieldNumber;
            this._fieldIdSet = true;
        }

        public override RuntimeTypeModel GetRuntimeTypeModel(RuntimeTypeModel protobufModel)
        {
            if (!this._fieldIdSet)
            {
                throw new SubclassFieldIdNotSetException("Field ID of subclass " + typeof(T).Name + " not set");
            }

            base.GetRuntimeTypeModel(protobufModel);

            var types = protobufModel.GetTypes().Cast<MetaType>();
            var baseType =
                types.SingleOrDefault(t => t.Type == typeof(T).BaseType);

            if (baseType != null && baseType.GetSubtypes().Any(s => s.FieldNumber == this._subclassFieldId))
            {
                throw new FieldIdAlreadyUsedException(this._subclassFieldId, baseType.GetSubtypes().First(s => s.FieldNumber == this._subclassFieldId).DerivedType);
            }

            if (baseType != null)
            {
                baseType.AddSubType(this._subclassFieldId, typeof(T));
            }

            return protobufModel;
        }

        public override bool CanBeResolvedUsing(RuntimeTypeModel protobufModel)
        {
            var types = protobufModel.GetTypes().Cast<MetaType>();
            var baseType =
                types.SingleOrDefault(t => t.Type == typeof(T).BaseType);

            return baseType != null;
        }

        public class SubclassFieldIdNotSetException : Exception
        {
            public SubclassFieldIdNotSetException(string message) : base(message)
            {
                
            }
        }
    }
}