using System;
using System.Runtime.Serialization;

namespace FluentProtobufNet.Tests.WCF
{
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
