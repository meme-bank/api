using OctopusAPI.Database.Entities.Core;

namespace OctopusAPI.Database.Entities.Trading
{
    public class ItemBlueprint
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string PhotoId { get; set; }
        public Photo Photo { get; set; }
        public List<Category> Categories { get; set; }
        public int Rarity { get; set; }
        public MeasuredIn MeasuredIn { get; set; }

        public int OwnerCopyrightId { get; set; } // id of the user that owned this item blueprint and can add copyrightids
        public int[] CopyrightIds { get; set; } // ids of the users that can use this item blueprint
        public bool OpenRecipe { get; set; } = true; // if true, anyone can use this item blueprint to craft items

        public ICollection<ItemBlueprintRecipe> Recipe { get; }
        public ICollection<ItemBlueprintRecipe> CraftableItems { get; } // items that can be crafted from this item

        public string CraftingStationId { get; set; } // id of the crafting station
        public ItemBlueprint CraftingStation { get; set; }
        public int CraftingTime { get; set; } // in seconds
        public int Health { get; set; } = 0; // for crafting stations
        public int CraftingStationWear { get; set; } // for craft on crafting stations

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class ItemBlueprintRecipe
    {
        public string RecipeItemId { get; set; }
        public ItemBlueprint RecipeItem { get; set; }
        public string CraftItemId { get; set; }
        public ItemBlueprint CraftItem { get; set; }
        public decimal Amount { get; set; }
    }

    public enum MeasuredIn
    {
        Pieces,
        Weight,
        Volume,
    }
}