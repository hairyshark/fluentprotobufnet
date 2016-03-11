using FluentProtobufNet.Mapping;
using NUnit.Framework;
using System.Linq;
using ProtoBuf.Meta;

namespace FluentProtobufNet.Tests
{
    [TestFixture]
    public class DevelopmentTests
    {
        private Configuration _config;

        [SetUp]
        public void Setup()
        {
            var runtimeTypeModel = TypeModel.Create();

            _config = Fluently.ConfigureWith(runtimeTypeModel)
                .Mappings(m =>
                    m.FluentMappings.AddFromAssemblyOf<CategoryMap>())
                .BuildConfiguration();
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
            Assert.IsTrue(category.GetSubtypes()[0].DerivedType.Type == typeof(CategoryWithDescription));
        }

        [Test]
        public void CorrectlyMapsUpToThirdLevelSubclass()
        {
            var types = _config.RuntimeTypeModel.GetTypes().Cast<MetaType>();
            var categoryWithDescription =
                types.SingleOrDefault(t => t.Type == typeof(CategoryWithDescription));

            Assert.IsNotNull(categoryWithDescription);
            Assert.IsTrue(categoryWithDescription.HasSubtypes);
            Assert.IsTrue(categoryWithDescription.GetSubtypes()[0].DerivedType.Type == typeof(CategoryThirdLevel));
        }
    }

    public class CategoryMap : ClassMap<Category>
    {
        public CategoryMap()
        {
            this.DynamicClassMap<CategoryMap, Category>();

//            Map(m => m.Name, 1);
//            Map(m => m.SubCategories, 2);
//            Map(m => m.Items, 3);
//            References(m => m.ParentCategory, 4);
        }
    }

    public interface IDynamicMapper
    {
        void Map();
    }

    public class CategoryWithDescriptionMap : SubclassMap<CategoryWithDescription>
    {
        public CategoryWithDescriptionMap()
        {
            this.DynamicSubclassMap<CategoryWithDescriptionMap, CategoryWithDescription>(1);

//            SubclassFieldId(1);
//            Map(c => c.Description, 1);
        }
    }

    public class CategoryThirdLevelMap : SubclassMap<CategoryThirdLevel>
    {
        public CategoryThirdLevelMap()
        {
            this.DynamicSubclassMap<CategoryThirdLevelMap, CategoryThirdLevel>(2);
//            SubclassFieldId(2);
//            Map(c => c.ThirdLevel, 1);
        }
    }

    public class ItemMap : ClassMap<Item>
    {
        public ItemMap()
        {
            this.DynamicClassMap<ItemMap, Item>();
//            Map(m => m.SKU, 1);
        }
    }


}
