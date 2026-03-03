using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MembankCore.Domain.Entities.Trading;

// То, что в инвентаре
public class Item
{
  public Guid ItemBlueprintId { get; private set; }
  public virtual ItemBlueprint ItemBlueprint { get; private set; }

  public int OwnerId { get; private set; }

  public DateTime OwnedAt { get; private set; }
  public decimal Amount { get; private set; }
}
