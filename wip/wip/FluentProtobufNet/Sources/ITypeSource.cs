using System;
using System.Collections.Generic;

namespace FluentProtobufNet.Sources
{
    public interface ITypeSource
    {
        IEnumerable<Type> GetTypeSources();

        string GetIdentifier();
    }
}