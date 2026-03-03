using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MembankCore.Domain.Entities.Core;

namespace MembankCore.Domain.Entities.Trading;

public class ItemBlueprint
{
  // Списки
  private readonly List<Category> _categories = new();
  private readonly List<ItemBlueprintRecipe> _recipe = new();
  private readonly List<int> _copyrightIds = new();

  // Сам объект
  public Guid Id { get; private set; }
  public string Name { get; private set; }
  public string Description { get; private set; }

  // Авторское право
  public int OwnerCopyrightId { get; private set; }
  public IReadOnlyCollection<int> CopyrightIds => _copyrightIds.AsReadOnly();
  public bool IsOpenRecipe { get; private set; } = true;

  // Крафт
  public string? CraftingStationId { get; private set; } // id of the crafting station
  public ItemBlueprint? CraftingStation { get; private set; }
  public int? CraftingTime { get; private set; } // in seconds
  public int? Health { get; private set; } = 0; // for crafting stations
  public int? CraftingStationWear { get; private set; } // for craft on crafting stations
  public IReadOnlyCollection<ItemBlueprintRecipe> Recipe => _recipe.AsReadOnly();
  public virtual IReadOnlyCollection<ItemBlueprintRecipe> CraftableItems { get; private set; } = []; // EF Core сам там всё сделает, он молодец

  // Фото
  public Guid PhotoId { get; private set; }
  public virtual Photo Photo { get; private set; }

  // Метаданные
  public DateTime CreatedAt { get; private set; }
  public IReadOnlyCollection<Category> Categories => _categories.AsReadOnly();
  public int Rarity { get; private set; }
  public MeasuredIn MeasuredIn { get; private set; } = MeasuredIn.Pieces; // how this item is measured, for example: pieces, weight, volume (if not pieces then it should be a decimal value)

  protected ItemBlueprint() { }

  public ItemBlueprint(string name, string description, int ownerId, Photo photo)
  {
    Id = Guid.NewGuid();
    Name = name;
    Description = description;
    OwnerCopyrightId = ownerId;
    Photo = photo;
    PhotoId = photo.Id;
    IsOpenRecipe = true;
    MeasuredIn = MeasuredIn.Pieces;
  }

  public bool IsUsableBy(int userId)
  {
    if (IsOpenRecipe) return true;
    if (userId == OwnerCopyrightId) return true;
    return _copyrightIds.Contains(userId);
  }

  public void AddToRecipe(ItemBlueprint ingredient, decimal amount)
  {
    if (ingredient.Id == this.Id)
      throw new Exception("Предмет не может требовать самого себя в рецепте.");

    var existing = _recipe.FirstOrDefault(r => r.RecipeItemId == ingredient.Id);
    if (existing != null)
      existing.UpdateAmount(amount);
    else
      _recipe.Add(new ItemBlueprintRecipe(ingredient, this, amount));
  }

  public void GrantCopyright(int ownerId, int targetUserId)
  {
    if (ownerId != OwnerCopyrightId)
      throw new Exception("Только владелец авторских прав может давать доступ.");

    if (!_copyrightIds.Contains(targetUserId))
      _copyrightIds.Add(targetUserId);
  }
}

public class ItemBlueprintRecipe
{
  public Guid RecipeItemId { get; private set; }
  public virtual ItemBlueprint RecipeItem { get; private set; }

  public Guid CraftItemId { get; private set; }
  public virtual ItemBlueprint CraftItem { get; private set; }

  public decimal Amount { get; private set; }

  protected ItemBlueprintRecipe() { }

  internal ItemBlueprintRecipe(ItemBlueprint ingredient, ItemBlueprint result, decimal amount)
  {
    RecipeItem = ingredient;
    RecipeItemId = ingredient.Id;
    CraftItem = result;
    CraftItemId = result.Id;
    Amount = amount;
  }

  internal void UpdateAmount(decimal newAmount) => Amount = newAmount;
}

public enum MeasuredIn
{
  Pieces,
  Weight,
  Volume,
}
