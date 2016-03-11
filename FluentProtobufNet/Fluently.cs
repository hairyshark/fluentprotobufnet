using ProtoBuf.Meta;

namespace FluentProtobufNet
{
    public class Fluently
    {
        public static FluentConfiguration Configure()
        {
            return new FluentConfiguration();
        }

        public static FluentConfiguration ConfigureWith(RuntimeTypeModel runtimeTypeModel)
        {
            return new FluentConfiguration(runtimeTypeModel);
        }

    }
}