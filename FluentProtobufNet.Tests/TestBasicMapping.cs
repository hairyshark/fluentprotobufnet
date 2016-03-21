using System.Collections.Generic;
using FluentProtobufNet.Configuration;
using FluentProtobufNet.Mapping;
using FluentProtobufNet.Sources;
using FluentProtobufNet.Tests.Basic;

namespace FluentProtobufNet.Tests
{
    using System;
    using System.Linq;

    using FluentProtobufNet.Specification;
    using FluentProtobufNet.Tests.Proto;

    using NUnit.Framework;

    using ProtoBuf.Meta;

    [TestFixture]
    public class TestBasicMapping
    {
        [SetUp]
        public void Setup()
        {
            this._model = TypeModel.Create();

            this._config =
                Fluently.Configure()
                    .WithModel(this._model)
                    .WithIndexor(SeededIndexor.GetIndex)
                    .Mappings(m => m.FluentMappings.AddFromAssemblySource<TestBasicMapping, AssemblyTypeSource<NameSpaceSpecification<Category>>>())
                    .BuildConfiguration();

            this.ShowResults();
        }

        private Configuration.Configuration _config;
        private IEnumerable<MetaType> _modelTypes;

        private RuntimeTypeModel _model;

        [Test]
        public void CanBuildConfiguration()
        {
            Assert.IsNotNull(this._config.RuntimeTypeModel);
            Assert.Greater(this._config.RuntimeTypeModel.GetTypes().Cast<object>().Count(), 0);
        }

        [Test]
        public void CorrectlyMapsSingleLevelSubclasses()
        {
            var category = this._modelTypes.SingleOrDefault(t => t.Type == typeof(Category));

            Assert.IsNotNull(category);
            Assert.IsTrue(category.HasSubtypes);
            Assert.IsTrue(category.GetSubtypes()[0].DerivedType.Type == typeof(CategoryWithDescription));
        }

        [Test]
        public void CorrectlyMapsUpToThirdLevelSubclass()
        {
            var categoryWithDescription = this._modelTypes.SingleOrDefault(t => t.Type == typeof(CategoryWithDescription));

            Assert.IsNotNull(categoryWithDescription);
            Assert.IsTrue(categoryWithDescription.HasSubtypes);
            Assert.IsTrue(categoryWithDescription.GetSubtypes()[0].DerivedType.Type == typeof(CategoryThirdLevel));
        }

        private void ShowResults()
        {
            this._modelTypes = this._config.RuntimeTypeModel.GetTypes().Cast<MetaType>();

            this._modelTypes.PrintSchemas(this._config);
        }

        [Test]
        public void TestRoundTripBaseType()
        {
            var obj = new Category()
            {
                Name = "Foo",
                ParentCategory = new CategoryWithDescription { Name = "Bar", Description = "ScoobyDoo" },
                Items = new[]
                            {
                                new Item { Definition = "abc", MainCategory = new CategoryThirdLevel { Description = "bananas" } }, 
                                new Item { Definition = "def", MainCategory = new CategoryThirdLevel { Description = "apples" } }, 
                                new Item { Definition = "ghi", MainCategory = new CategoryThirdLevel { Description = "oranges" } }
                            }
            };

            var clone = obj.RoundTrip(this._model);

            Assert.IsNotNull(clone);
            var subclass = clone.ParentCategory as CategoryWithDescription;

            Assert.AreEqual("Foo", clone.Name);
            Assert.AreEqual("Bar", clone.ParentCategory.Name);
            Assert.IsNotNull(subclass);
            Assert.AreEqual("ScoobyDoo", subclass.Description);

            Assert.AreEqual(3, clone.Items.Count);
        }
    }
}