using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MembankCore.Domain.Entities.Economic;
using MembankCore.Domain.ValueObjects;

namespace MembankCore.Domain.Entities.Trading;

public class ProvideService {
  public Guid ServiceId { get; private set; }
  public virtual Service Service { get; private set; }

  public int BuyerId { get; private set; }

  public Money? Price { get; private set; }
  public virtual Currency? Currency { get; private set; }
  public string? CurrencyId => Price.CurrencyId;

  public ProvideStatus Status { get; private set; } = ProvideStatus.Active;

  public DateTime StartAt { get; private set; }
  public DateTime? ExpiresAt { get; private set; }
}

public enum ProvideStatus {
  Active,
  Succesess, // Only for once services
  Cancel,
  Expired
}
