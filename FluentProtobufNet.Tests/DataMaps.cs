using FluentProtobufNet.Helpers;
using FluentProtobufNet.Mapping;

namespace FluentProtobufNet.Tests
{
    public class CategoryMap : ClassMap<Category>
    {
        public CategoryMap()
        {
            this.DynamicClassMap<CategoryMap, Category>(Indexor.GetIndex);
        }
    }

    public class CategoryWithDescriptionMap : SubclassMap<CategoryWithDescription>
    {
        public CategoryWithDescriptionMap()
        {
            this.DynamicSubclassMap<CategoryWithDescriptionMap, CategoryWithDescription>(1, Indexor.GetIndex);
        }
    }

    public class CategoryThirdLevelMap : SubclassMap<CategoryThirdLevel>
    {
        public CategoryThirdLevelMap()
        {
            this.DynamicSubclassMap<CategoryThirdLevelMap, CategoryThirdLevel>(2, Indexor.GetIndex);
        }
    }

    public class ItemMap : ClassMap<Item>
    {
        public ItemMap()
        {
            this.DynamicClassMap<ItemMap, Item>(Indexor.GetIndex);
        }
    }
}
