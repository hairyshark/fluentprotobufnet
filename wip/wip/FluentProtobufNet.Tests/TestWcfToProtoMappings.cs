using System;
using System.Reflection;
using FluentProtobufNet.Mapping;
using FluentProtobufNet.Tests.WCF;
using NUnit.Framework;
using ProtoBuf.Meta;

namespace FluentProtobufNet.Tests
{
    [TestFixture]
    public class TestWcfToProtoMappings
    {
        [SetUp]
        public void SetUp()
        {
            this._model = TypeModel.Create();

            this._model.AutoCompile = false;
        }

        private RuntimeTypeModel _model;

        private readonly DateTime _testDate = new DateTime(1999, 12, 31);

        [Test(Description = "this shows how datacontracts fail by default")]
        public void RuntimeInheritanceFailsVanillaAndNoInferredNames()
        {
            this._model.InferTagFromNameDefault = false;

            var t0 = typeof (TradeVanilla);
            var t1 = typeof (IRSwapVanilla);

            var abst = this._model.Add(t0, true);

            // define inheritance here...
            abst.AddSubType(100, t1);

            Console.WriteLine(this._model.GetSchema(typeof (TradeVanilla)));
            Console.WriteLine(this._model.GetSchema(typeof (IRSwapVanilla)));

            TradeVanilla foo = new IRSwapVanilla {SwapRate = 75, TradeDateTime = this._testDate};

            var bar = (TradeVanilla) this._model.DeepClone(foo);
            var cloneSwap = bar as IRSwapVanilla;

            Assert.AreEqual(DateTime.MinValue, bar.TradeDateTime);
            Assert.IsNotNull(cloneSwap);
            Assert.AreEqual(0, cloneSwap.SwapRate);
        }

        [Test(
            Description =
                "map from datacontracts into protobuf contracts meets our requirement with no inferred names BUT DOESN'T WORK FOR PROTOBUF-NET REASONS"
            )]
        [ExpectedException(typeof (InvalidOperationException),
            ExpectedMessage =
                "Type is not expected, and no contract can be inferred: FluentProtobufNet.Tests.WCF.IRSwapVanilla")]
        public void RuntimeInheritanceFailsVanillaWithCustomReflectionWithNoInferredNames()
        {
            this._model.InferTagFromNameDefault = false;
            this._model.AutoAddMissingTypes = true;
            this._model.AutoAddProtoContractTypesOnly = true;

            var ctx = new DataContractReflectionContext();

            var t0 = ctx.MapType(typeof (TradeVanilla).GetTypeInfo());
            var t1 = ctx.MapType(typeof (IRSwapVanilla).GetTypeInfo());
            var t2 = ctx.MapType(typeof (SubclassVanilla1).GetTypeInfo());
            var t3 = ctx.MapType(typeof (SubclassVanilla2).GetTypeInfo());

            var abst = this._model.Add(t0, true);

            // define inheritance here...
            abst.AddSubType(100, t1);
            abst.AddSubType(101, t2);
            abst.AddSubType(102, t3);

            TradeVanilla foo = new IRSwapVanilla {SwapRate = 75, TradeDateTime = this._testDate};

            var bar = (TradeVanilla) this._model.DeepClone(foo);

            var cloneSwap = bar as IRSwapVanilla;

            Assert.AreEqual(this._testDate, bar.TradeDateTime);
            Assert.IsNotNull(cloneSwap);
            Assert.AreEqual(75, cloneSwap.SwapRate);

            Console.WriteLine(this._model.GetSchema(typeof (TradeVanilla)));
            Console.WriteLine(this._model.GetSchema(typeof (IRSwapVanilla)));
        }

        [Test(
            Description =
                "this shows how datacontracts can be used with an auto tagging of properties. NO GOOD FOR US AS WE WANT A FIXED TAG PER PROPERTY"
            )]
        public void RuntimeInheritancePassesVanillaAndInferredNames()
        {
            this._model.InferTagFromNameDefault = true;

            var t0 = typeof (TradeVanilla);
            var t1 = typeof (IRSwapVanilla);

            var abst = this._model.Add(t0, true);

            // define inheritance here...
            abst.AddSubType(100, t1);

            Console.WriteLine(this._model.GetSchema(typeof (TradeVanilla)));
            Console.WriteLine(this._model.GetSchema(typeof (IRSwapVanilla)));

            TradeVanilla foo = new IRSwapVanilla {SwapRate = 75, TradeDateTime = this._testDate};

            var bar = (TradeVanilla) this._model.DeepClone(foo);
            var cloneSwap = bar as IRSwapVanilla;

            Assert.AreEqual(this._testDate, bar.TradeDateTime);
            Assert.IsNotNull(cloneSwap);
            Assert.AreEqual(75, cloneSwap.SwapRate);
        }
    }
}