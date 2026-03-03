using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MembankCore.Domain.Entities.Economic;
using MembankCore.Domain.ValueObjects;

namespace MembankCore.Domain.Entities.Trading;

public class Product
{
  public Guid ItemBlueprintId { get; private set; }
  public virtual ItemBlueprint ItemBlueprint { get; private set; }

  public int SellerId { get; private set; }

  public Money Price { get; set; }
  public string CurrencyId => Price.CurrencyId;
  public virtual Currency Currency { get; private set; }

  public decimal Amount { get; private set; }
}
