using System;
using System.Runtime.Serialization;
using ProtoBuf;

namespace FluentProtobufNet.Tests.WCF
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

    [DataContract]
    public class ExecutionVanilla
    {
        [DataMember]
        public int OrderId { get; set; }

        [DataMember]
        public TradeVanilla MyTradeVanilla { get; set; }

        public string IgnoreMe { get; set; }
    }

    [KnownType(typeof(IRSwapVanilla))]
    [KnownType(typeof(SubclassVanilla1))]
    [KnownType(typeof(SubclassVanilla2))]
    [DataContract]
    public abstract class TradeVanilla
    {
        [DataMember]
        public DateTime TradeDateTime { get; set; }
    }

    [DataContract]
    public class IRSwapVanilla : TradeVanilla
    {
        [DataMember]
        public int SwapRate { get; set; }
    }

    [DataContract]
    public class SubclassVanilla1 : TradeVanilla
    {
        [DataMember]
        public int Foo1 { get; set; }
    }

    [DataContract]
    public class SubclassVanilla2 : TradeVanilla
    {
        [DataMember]
        public int Foo2 { get; set; }
    }
}
