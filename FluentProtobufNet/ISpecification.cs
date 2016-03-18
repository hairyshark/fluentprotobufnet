using System;

namespace FluentProtobufNet
{
    public interface ISpecification
    {
        bool IsSatisfied(Type type);
    }
}