using FluentProtobufNet.Tests.Basic;

namespace FluentProtobufNet.Tests
{
    using System.Linq;

    using NUnit.Framework;

    using ProtoBuf.Meta;

    [TestFixture]
    public class TestBasicMapping
    {
        [SetUp]
        public void Setup()
        {
            var runtimeTypeModel = TypeModel.Create();

            this._config =
                Fluently.Configure()
                    .WithModel(runtimeTypeModel)
                    .WithIndexor(Indexor.GetIndex)
                    .Mappings(m => m.FluentMappings.AddFromAssemblySource<TestBasicMapping, AssemblyTypeSource<NameSpaceSpecification<Category>>>())
                    .BuildConfiguration();
        }

        private Configuration _config;

        [Test]
        public void CanBuildConfiguration()
        {
            Assert.IsNotNull(this._config.RuntimeTypeModel);
            Assert.Greater(this._config.RuntimeTypeModel.GetTypes().Cast<object>().Count(), 0);
        }

        [Test]
        public void CorrectlyMapsSingleLevelSubclasses()
        {
            var types = this._config.RuntimeTypeModel.GetTypes().Cast<MetaType>();
            var category = types.SingleOrDefault(t => t.Type == typeof(Category));

            Assert.IsNotNull(category);
            Assert.IsTrue(category.HasSubtypes);
            Assert.IsTrue(category.GetSubtypes()[0].DerivedType.Type == typeof(CategoryWithDescription));
        }

        [Test]
        public void CorrectlyMapsUpToThirdLevelSubclass()
        {
            var types = this._config.RuntimeTypeModel.GetTypes().Cast<MetaType>();
            var categoryWithDescription = types.SingleOrDefault(t => t.Type == typeof(CategoryWithDescription));

            Assert.IsNotNull(categoryWithDescription);
            Assert.IsTrue(categoryWithDescription.HasSubtypes);
            Assert.IsTrue(categoryWithDescription.GetSubtypes()[0].DerivedType.Type == typeof(CategoryThirdLevel));
        }
    }
}