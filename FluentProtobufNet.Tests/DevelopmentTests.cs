using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Context;
using System.Runtime.Serialization;
using NUnit.Framework;
using ProtoBuf;
using ProtoBuf.Meta;

namespace FluentProtobufNet.Tests
{
    [TestFixture]
    public class DevelopmentTests
    {
        [SetUp]
        public void Setup()
        {
            var runtimeTypeModel = TypeModel.Create();

            _config = Fluently.ConfigureWith(runtimeTypeModel)
                .Mappings(m =>
                    m.FluentMappings.AddFromAssemblyOf<CategoryMap>())
                .BuildConfiguration();
        }

        private Configuration _config;

        public class MyProtoExtenderContext : CustomReflectionContext
        {
            protected override IEnumerable<PropertyInfo> AddProperties(Type type)
            {
                if (type.GetCustomAttributes<DataContractAttribute>(true).FirstOrDefault() == null)
                    return base.AddProperties(type);

                var fakeProp = CreateProperty(MapType(typeof (string).GetTypeInfo()).AsType(), "FakeProperty",
                    o => "FakeValue", (o, v) => Console.WriteLine("Setting value = " + v));

                return new[] {fakeProp};
            }

            protected override IEnumerable<object> GetCustomAttributes(MemberInfo member,
                IEnumerable<object> declaredAttributes)
            {
                var dc = FirstOrDefault<DataContractAttribute>(member);
                var dm = FirstOrDefault<DataMemberAttribute>(member);

                if (dc != null)
                {
                    var protoAttr = new ProtoContractAttribute { InferTagFromName = true};

                    return new[] { protoAttr };
                }

                if (dm != null)
                {
                    var protoAttr = new ProtoMemberAttribute(member.GetIndex());

                    return new[] { protoAttr };
                }

                return base.GetCustomAttributes(member, declaredAttributes);
            }

            private static TAttribute FirstOrDefault<TAttribute>(MemberInfo member) where TAttribute : Attribute
            {
                return member.GetCustomAttributes<TAttribute>(true).FirstOrDefault();
            }
        }

        [Test]
        public void AddProtoAttributesToDataContracts()
        {
            var ctx = new MyProtoExtenderContext();

            var mappedType = ctx.MapType(typeof (ToyBox).GetTypeInfo());

            var declaredProperties = mappedType.DeclaredProperties;

            var propertyInfos = declaredProperties as IList<PropertyInfo> ?? declaredProperties.ToList();

            foreach (var prop in propertyInfos)
            {
                Console.WriteLine(prop.Name);
            }

            foreach (Attribute attr in mappedType.GetCustomAttributes(true))
            {
                Console.WriteLine(attr);
            }

            foreach (var info in propertyInfos)
            {
                foreach (var attr in info.GetCustomAttributes<ProtoMemberAttribute>(true))
                {
                    Console.WriteLine(attr.ToString());
                    Console.WriteLine(attr.Tag);
                    Console.WriteLine(attr.Name);
                }
            }
        }

        [Test]
        public void CanBuildConfiguration()
        {
            Assert.IsNotNull(_config.RuntimeTypeModel);
            Assert.Greater(_config.RuntimeTypeModel.GetTypes().Cast<object>().Count(), 0);
        }

        [Test]
        public void CorrectlyMapsSingleLevelSubclasses()
        {
            var types = _config.RuntimeTypeModel.GetTypes().Cast<MetaType>();
            var category =
                types.SingleOrDefault(t => t.Type == typeof (Category));

            Assert.IsNotNull(category);
            Assert.IsTrue(category.HasSubtypes);
            Assert.IsTrue(category.GetSubtypes()[0].DerivedType.Type == typeof (CategoryWithDescription));
        }

        [Test]
        public void CorrectlyMapsUpToThirdLevelSubclass()
        {
            var types = _config.RuntimeTypeModel.GetTypes().Cast<MetaType>();
            var categoryWithDescription =
                types.SingleOrDefault(t => t.Type == typeof (CategoryWithDescription));

            Assert.IsNotNull(categoryWithDescription);
            Assert.IsTrue(categoryWithDescription.HasSubtypes);
            Assert.IsTrue(categoryWithDescription.GetSubtypes()[0].DerivedType.Type == typeof (CategoryThirdLevel));
        }
    }
}