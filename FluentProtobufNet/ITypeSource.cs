using System;
using System.Collections.Generic;

namespace FluentProtobufNet
{
    public interface ITypeSource
    {
        IEnumerable<Type> GetTypes();

        string GetIdentifier();
    }
}