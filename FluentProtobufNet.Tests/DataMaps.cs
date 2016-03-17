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
        public CategoryWithDescriptionMap(int discriminator)
        {
            this.DynamicSubclassMap<CategoryWithDescriptionMap, CategoryWithDescription>(discriminator, Indexor.GetIndex);
        }
    }

    public class CategoryThirdLevelMap : SubclassMap<CategoryThirdLevel>
    {
        public CategoryThirdLevelMap(int discriminator)
        {
            this.DynamicSubclassMap<CategoryThirdLevelMap, CategoryThirdLevel>(discriminator, Indexor.GetIndex);
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
