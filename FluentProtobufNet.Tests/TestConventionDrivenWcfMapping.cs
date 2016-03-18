using System;
using System.Linq;
using FluentProtobufNet.Tests.WCF;
using NUnit.Framework;
using ProtoBuf.Meta;

namespace FluentProtobufNet.Tests
{
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
                    .WithIndexor(Indexor.GetIndex)
                    .Mappings(
                        m =>
                        {
                            m.FluentMappings.AddFromAssemblySource<TestConventionDrivenWcfMapping, WcfAssemblyTypeSource<NameSpaceSpecification<ExecutionVanilla>>>();
                        })
                    .BuildConfiguration();
        }

        private Configuration _config;

        private RuntimeTypeModel _model;

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
    }
}