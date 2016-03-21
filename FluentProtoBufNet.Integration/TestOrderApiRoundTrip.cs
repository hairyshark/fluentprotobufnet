﻿using Insight.Messaging.OTC.OrderAPI.BusinessEntities;

namespace TradeReaderService
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using FluentProtobufNet;
    using FluentProtobufNet.Mapping;
    using FluentProtobufNet.Specification;
    using FluentProtobufNet.Tests;

    using Insight.Messaging.OTC.OrderAPI;
    using Insight.Messaging.OTC.OrderAPI.Data.Contract;

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
                    .WithIndexor(Indexor.GetIndex)
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

            Assert.IsNotNull(dto);

            var clone = dto.RoundTrip(this._model);

            Assert.IsNotNull(clone);
            Assert.IsNotNull(clone.ContractData);
            Assert.AreEqual(dto.ContractData, clone.ContractData);
        }
    }
}