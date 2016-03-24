using System;
using ProtoBuf;

namespace FluentProtobufNet.Tests.Proto
{
    [ProtoContract]
    public class ExecutionProto
    {
        [ProtoMember(2)]
        public int OrderId { get; set; }

        [ProtoMember(1)]
        public TradeProto MyTradeProto { get; set; }
    }

    [ProtoContract]
    [ProtoInclude(1, typeof(IRSwapProto))]
    public abstract class TradeProto
    {
        [ProtoMember(3)]
        public DateTime TradeDateTime { get; set; }
    }

    [ProtoContract]
    public class IRSwapProto : TradeProto
    {
        [ProtoMember(4)]
        public int SwapRate { get; set; }
    }

}
