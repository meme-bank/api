using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MembankCore.Domain.Entities.Core;

namespace MembankCore.Domain.Entities.Trading;

public class Category {
  private readonly List<Service> _services = [];
  private readonly List<ItemBlueprint> _itemBlueprints = [];

  public Guid Id { get; private set; }
  public string Name { get; private set; }
  public Photo? Icon { get; private set; } // Monochrome

  public virtual IReadOnlyCollection<Service> Services => _services.AsReadOnly();
  public virtual IReadOnlyCollection<ItemBlueprint> ItemBlueprints => _itemBlueprints.AsReadOnly();

  public Category(string name, Photo? icon = null) {
    if (string.IsNullOrWhiteSpace(name))
      throw new Exception("Название категории не может быть пустым.");

    Id = Guid.NewGuid();
    Name = name;
    Icon = icon;
  }
}
