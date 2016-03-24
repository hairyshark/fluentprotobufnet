namespace TradeReaderService
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.Serialization;
    using System.Text;

    using FluentProtobufNet.Configuration;
    using FluentProtobufNet.Mapping;
    using FluentProtobufNet.Sources;
    using FluentProtobufNet.Specification;
    using FluentProtobufNet.Tests;

    using Insight.Messaging.OTC.OrderAPI;

    using NUnit.Framework;

    using ProtoBuf;
    using ProtoBuf.Meta;

    [TestFixture]
    public class TestOrderApiRoundTrip
    {
        [SetUp]
        public void SetUp()
        {
            this._model = TypeModel.Create();

            this._config =
                Fluently.Configure()
                    .WithModel(this._model)
                    .WithIndexor(SeededIndexor.GetIndex)
                    .Mappings(
                        m =>
                            {
                                m.FluentMappings.AddFromAssemblySource<ExecutionNew, WcfAssemblyTypeSource<AnyDataContractSpecification>>();
//                                m.FluentMappings.AddFromAssemblySource<ExecutionNew, WcfAssemblyTypeSource<SpecificDataContractSpecification<FXOptionDoubleBarrierDigital>>>();
//                                m.FluentMappings.AddFromAssemblySource<ExecutionNew, WcfAssemblyTypeSource<SpecificDataContractSpecification<FXOptionDoubleBarrier>>>();
//                                m.FluentMappings.AddFromAssemblySource<ExecutionNew, WcfAssemblyTypeSource<SpecificDataContractSpecification<FXOptionDoubleBarrierBase>>>();
//                                m.FluentMappings.AddFromAssemblySource<ExecutionNew, WcfAssemblyTypeSource<SpecificDataContractSpecification<FXOptionBarrierBase>>>();
//                                m.FluentMappings.AddFromAssemblySource<ExecutionNew, WcfAssemblyTypeSource<SpecificDataContractSpecification<FXOptionBase>>>();
//                                m.FluentMappings.AddFromAssemblySource<ExecutionNew, WcfAssemblyTypeSource<SpecificDataContractSpecification<ContractBlockBase>>>();
//                                m.FluentMappings.AddFromAssemblySource<ExecutionNew, WcfAssemblyTypeSource<SpecificDataContractSpecification<ContractBase>>>();
//                                m.FluentMappings.AddFromAssemblySource<ExecutionNew, WcfAssemblyTypeSource<SpecificDataContractSpecification<BusinessEntity>>>();
                            })
                    .BuildConfiguration();
        }

        private Configuration _config;

        private RuntimeTypeModel _model;

        [Test]
        public void CanBuildConfiguration()
        {
            Assert.IsNotNull(this._config.RuntimeTypeModel);
            Assert.Greater(this._config.RuntimeTypeModel.GetTypes().Cast<object>().Count(), 0);

            var schema = this._model.GetSchema(typeof(ExecutionNew));

            Assert.IsNotNull(schema);

            Console.WriteLine(schema);
        }

        [Test]
        public void TestVisitWcfVanillaDataContracts()
        {
            typeof(ExecutionNew).GetTypeInfo()
                .Visit<DataContractAttribute, DataMemberAttribute, KnownTypeAttribute>();
        }

        [Test]
        public void TestVisitWcfVanillaGeneratesProtoContracts()
        {
            var ctx = new DataContractReflectionContext();

            ctx.MapType(typeof(ExecutionNew).GetTypeInfo())
                .Visit<ProtoContractAttribute, ProtoMemberAttribute, ProtoIncludeAttribute>();
        }

        [Test]
        [Ignore]
        public void TestRoundTripExecutionNew()
        {
            var reader = new OrderApiReader();

            var dto = reader.Load<ExecutionNew>(222723);

            var clone = dto.RoundTrip(this._model);

            Assert.IsNotNull(clone);
            Assert.IsNotNull(clone.ContractData);
        }

        [Test]
        [Ignore]
        public void TestRoundTripExecutionNewWithDataContractSerializer()
        {
            var reader = new OrderApiReader();

            var dto = reader.Load<ExecutionNew>(222723);

            Assert.IsNotNull(dto);

            using (var ms = new MemoryStream())
            using (var ms2 = new MemoryStream())
            {
                var serliazer = new DataContractSerializer(typeof(ExecutionNew));

                serliazer.WriteObject(ms, dto);

                Debug.WriteLine(Encoding.UTF8.GetString(ms.GetBuffer(), 0, (int)ms.Length));

                ms.Position = 0;

                var clone = serliazer.ReadObject(ms) as ExecutionNew;

                Assert.IsNotNull(clone, "clone");

                serliazer.WriteObject(ms2, clone);

                Assert.AreEqual(ms, ms2);
            }
        }

        [Test]
        [Ignore]
        public void TestRoundTripExecutionNewWithDataContractSerializerNotEqual()
        {
            var reader = new OrderApiReader();

            var dto = reader.Load<ExecutionNew>(222723);

            Assert.IsNotNull(dto);

            using (var ms = new MemoryStream())
            using (var ms2 = new MemoryStream())
            {
                var dataContractSerializer = new DataContractSerializer(typeof(ExecutionNew));

                dataContractSerializer.WriteObject(ms, dto);

                Debug.WriteLine(Encoding.UTF8.GetString(ms.GetBuffer(), 0, (int)ms.Length));

                ms.Position = 0;

                var clone = dataContractSerializer.ReadObject(ms) as ExecutionNew;

                Assert.IsNotNull(clone, "clone");

                clone.OrderId = 999999; // lets change something

                dataContractSerializer.WriteObject(ms2, clone);

                Assert.AreNotEqual(ms, ms2);
            }
        }
    }
}