using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using FluentProtobufNet.Mapping;
using FluentProtobufNet.Tests.WCF;
using NUnit.Framework;
using ProtoBuf;
using ProtoBuf.Meta;

namespace FluentProtobufNet.Tests
{
    [TestFixture]
    public class TestBasicWcfMapping
    {
        [SetUp]
        public void Setup()
        {
            this._model = TypeModel.Create();

            this._config =
                Fluently.Configure()
                    .WithModel(this._model)
                    .WithIndexor(Indexor.GetIndex)
                    .Mappings(
                        m =>
                        {
                            m.FluentMappings.Add<WcfClassMap<ExecutionVanilla>>();
                            m.FluentMappings.Add<WcfClassMap<TradeVanilla>>();
                            m.FluentMappings.Add<WcfSubClassMap<IRSwapVanilla>>();
                            m.FluentMappings.Add<WcfSubClassMap<SubclassVanilla1>>();
                            m.FluentMappings.Add<WcfSubClassMap<SubclassVanilla2>>();
                        })
                    .BuildConfiguration();

            this.ShowResults();
        }

        private Configuration _config;

        private RuntimeTypeModel _model;
        private IEnumerable<MetaType> _modelTypes;

        [Test]
        public void TestCorrectlyMapsSingleLevelSubTypes()
        {
            var types = this._config.RuntimeTypeModel.GetTypes().Cast<MetaType>();

            var vanilla = types.SingleOrDefault(t => t.Type == typeof (TradeVanilla));

            Assert.IsNotNull(vanilla);
            Assert.IsTrue(vanilla.HasSubtypes);
            var subTypes = vanilla.GetSubtypes();

            Assert.AreEqual(subTypes.Length, 3);

            Assert.IsTrue(subTypes[0].DerivedType.Type == typeof (IRSwapVanilla));
            Assert.IsTrue(subTypes[1].DerivedType.Type == typeof (SubclassVanilla1));
            Assert.IsTrue(subTypes[2].DerivedType.Type == typeof (SubclassVanilla2));
        }

        [Test]
        public void TestProtoContractsPassRoundTrip()
        {
            var testDate = new DateTime(1999, 12, 31);

            var obj = new ExecutionProto
            {
                OrderId = 123,
                MyTradeProto = new IRSwapProto {TradeDateTime = testDate, SwapRate = 10}
            };

            var clone = obj.RoundTrip(this._model);

            Assert.AreEqual(123, clone.OrderId);
            Assert.AreEqual(testDate, clone.MyTradeProto.TradeDateTime);
            Assert.IsInstanceOf<IRSwapProto>(clone.MyTradeProto);

            var swap = clone.MyTradeProto as IRSwapProto;
            Assert.IsNotNull(swap);
            Assert.AreEqual(10, swap.SwapRate);
        }

        [Test]
        public void TestVisitWcfVanilla()
        {
            typeof (ExecutionVanilla).GetTypeInfo()
                .Visit<DataContractAttribute, DataMemberAttribute, KnownTypeAttribute>();
            typeof (TradeVanilla).GetTypeInfo().Visit<DataContractAttribute, DataMemberAttribute, KnownTypeAttribute>();
            typeof (IRSwapVanilla).GetTypeInfo().Visit<DataContractAttribute, DataMemberAttribute, KnownTypeAttribute>();
        }

        [Test]
        public void TestVisitWcfVanillaGeneratesProtoContracts()
        {
            var ctx = new DataContractReflectionContext();

            ctx.MapType(typeof (ExecutionVanilla).GetTypeInfo())
                .Visit<ProtoContractAttribute, ProtoMemberAttribute, ProtoIncludeAttribute>();
            ctx.MapType(typeof (TradeVanilla).GetTypeInfo())
                .Visit<ProtoContractAttribute, ProtoMemberAttribute, ProtoIncludeAttribute>();
            ctx.MapType(typeof (IRSwapVanilla).GetTypeInfo())
                .Visit<ProtoContractAttribute, ProtoMemberAttribute, ProtoIncludeAttribute>();
        }

        [Test]
        public void TestWcfVanillaContractsPassRoundTrip()
        {
            var testDate = new DateTime(1999, 12, 31);

            var obj = new ExecutionVanilla
            {
                OrderId = 123,
                MyTradeVanilla = new IRSwapVanilla {TradeDateTime = testDate, SwapRate = 10},
                IgnoreMe = "unimportant message"
            };

            var clone = obj.RoundTrip(this._model);

            Assert.AreEqual(123, clone.OrderId);
            Assert.AreEqual(testDate, clone.MyTradeVanilla.TradeDateTime);
            Assert.IsInstanceOf<IRSwapVanilla>(clone.MyTradeVanilla);

            var swap = clone.MyTradeVanilla as IRSwapVanilla;
            Assert.IsNotNull(swap);
            Assert.AreEqual(10, swap.SwapRate);
            Assert.IsNullOrEmpty(clone.IgnoreMe);
        }

        private void ShowResults()
        {
            this._modelTypes = this._config.RuntimeTypeModel.GetTypes().Cast<MetaType>();

            this._modelTypes.PrintSchemas(this._config);
        }
    }
}