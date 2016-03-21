using System;
using System.Collections.Generic;
using System.Linq;
using FluentProtobufNet.Configuration;
using FluentProtobufNet.Mapping;
using FluentProtobufNet.Sources;
using FluentProtobufNet.Tests.WCF;
using NUnit.Framework;
using ProtoBuf.Meta;

namespace FluentProtobufNet.Tests
{
    using FluentProtobufNet.Specification;

    [TestFixture]
    public class TestConventionDrivenWcfMapping
    {
        [SetUp]
        public void Setup()
        {
            this._model = TypeModel.Create();

            this._config =
                Fluently.Configure()
                    .WithModel(this._model)
                    .WithIndexor(SeededIndexor.GetIndex)
                    .Mappings(
                        m =>
                        {
                            m.FluentMappings.AddFromAssemblySource<TestConventionDrivenWcfMapping, WcfAssemblyTypeSource<NameSpaceSpecification<ExecutionVanilla>>>();
                        })
                    .BuildConfiguration();

            this.ShowResults();
        }

        private Configuration.Configuration _config;
        private RuntimeTypeModel _model;
        private IEnumerable<MetaType> _modelTypes;

        [Test]
        public void TestCorrectlyMapsSingleLevelSubTypes()
        {
            var vanilla = this._modelTypes.SingleOrDefault(t => t.Type == typeof(TradeVanilla));

            Assert.IsNotNull(vanilla);
            Assert.IsTrue(vanilla.HasSubtypes);
            var subTypes = vanilla.GetSubtypes();

            Assert.AreEqual(subTypes.Length, 3);

            Assert.IsTrue(subTypes[0].DerivedType.Type == typeof(IRSwapVanilla));
            Assert.IsTrue(subTypes[1].DerivedType.Type == typeof(SubclassVanilla1));
            Assert.IsTrue(subTypes[2].DerivedType.Type == typeof(SubclassVanilla2));
        }

        [Test(Description = "THIS TICKS ALL THE BOXES FOR DATACONTRACT SERLIAZATION THROUGH PROTOBUF")]
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