namespace FluentProtobufNet.Tests
{
    using System;
    using System.Reflection;

    using NUnit.Framework;

    using ProtoBuf.Meta;

    [TestFixture]
    public class TestWcfToProtoMappings
    {
        private RuntimeTypeModel model;

        private readonly DateTime testDate = new DateTime(1999, 12, 31);


        [SetUp]
        public void SetUp()
        {
            this.model = TypeModel.Create();

            this.model.AutoCompile = false;
        }

        [Test(Description = "map from datacontracts into protobuf contracts meet our requirement")]
        public void RuntimeInheritancePassesVanillaAndReflectionWithNoInferredNames()
        {
            this.model.InferTagFromNameDefault = false;

            var ctx = new DataContractReflectionContext();

            var t0 = ctx.MapType(typeof(TradeVanilla).GetTypeInfo());
            var t1 = ctx.MapType(typeof(IRSwapVanilla).GetTypeInfo());

            var abst = this.model.Add(t0, true);

            // define inheritance here...
            abst.AddSubType(100, t1);

            Console.WriteLine(this.model.GetSchema(typeof(TradeVanilla)));
            Console.WriteLine(this.model.GetSchema(typeof(IRSwapVanilla)));

            TradeVanilla foo = new IRSwapVanilla { SwapRate = 75, TradeDateTime = this.testDate };

            var bar = (TradeVanilla)this.model.DeepClone(foo);
            var cloneSwap = bar as IRSwapVanilla;

            Assert.AreEqual(this.testDate, bar.TradeDateTime);
            Assert.IsNotNull(cloneSwap);
            Assert.AreEqual(75, cloneSwap.SwapRate);
        }

        [Test(Description = "this shows how datacontracts fail by default")]
        public void RuntimeInheritanceFailsVanillaAndNoInferredNames()
        {
            this.model.InferTagFromNameDefault = false;

            var t0 = typeof(TradeVanilla);
            var t1 = typeof(IRSwapVanilla);

            var abst = this.model.Add(t0, true);

            // define inheritance here...
            abst.AddSubType(100, t1);

            Console.WriteLine(this.model.GetSchema(typeof(TradeVanilla)));
            Console.WriteLine(this.model.GetSchema(typeof(IRSwapVanilla)));

            TradeVanilla foo = new IRSwapVanilla { SwapRate = 75, TradeDateTime = this.testDate };

            var bar = (TradeVanilla)this.model.DeepClone(foo);
            var cloneSwap = bar as IRSwapVanilla;

            Assert.AreEqual(DateTime.MinValue, bar.TradeDateTime);
            Assert.IsNotNull(cloneSwap);
            Assert.AreEqual(0, cloneSwap.SwapRate);
        }

        [Test(Description = "this shows how datacontracts can be used with an auto tagging of properties. NO GOOD FOR US AS WE WANT A FIXED TAG PER PROPERTY")]
        public void RuntimeInheritancePassesVanillaAndInferredNames()
        {
            this.model.InferTagFromNameDefault = true;

            var t0 = typeof(TradeVanilla);
            var t1 = typeof(IRSwapVanilla);

            var abst = this.model.Add(t0, true);

            // define inheritance here...
            abst.AddSubType(100, t1);

            Console.WriteLine(this.model.GetSchema(typeof(TradeVanilla)));
            Console.WriteLine(this.model.GetSchema(typeof(IRSwapVanilla)));

            TradeVanilla foo = new IRSwapVanilla { SwapRate = 75, TradeDateTime = this.testDate };

            var bar = (TradeVanilla)this.model.DeepClone(foo);
            var cloneSwap = bar as IRSwapVanilla;

            Assert.AreEqual(this.testDate, bar.TradeDateTime);
            Assert.IsNotNull(cloneSwap);
            Assert.AreEqual(75, cloneSwap.SwapRate);
        }
    }
}
