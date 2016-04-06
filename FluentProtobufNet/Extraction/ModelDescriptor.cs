using ProtoBuf.Meta;

namespace FluentProtobufNet.Extraction
{
    public class ModelDescriptor
    {
        public ModelDescriptor(MetaType metaType, int index)
        {
            this.MetaType = metaType;
            this.Index = index;
        }

        public int Index { get; private set; }

        public MetaType MetaType { get; private set; }
    }
}