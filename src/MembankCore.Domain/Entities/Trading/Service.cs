using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MembankCore.Domain.Entities.Core;
using MembankCore.Domain.Entities.Economic;
using MembankCore.Domain.ValueObjects;

namespace MembankCore.Domain.Entities.Trading;

public class Service
{
  private readonly List<Category> _catgories = [];
  private readonly List<ProvideService> _provideServices = [];

  public Guid Id { get; private set; }
  public string Name { get; private set; }
  public string Description { get; private set; }
  public Guid PhotoId { get; private set; }
  public virtual Photo Photo { get; private set; }

  public ServiceType Type { get; private set; } = ServiceType.Once;
  public int ProviderId { get; private set; }

  public Money? Price { get; private set; }
  public string? CurrencyId => Price.CurrencyId;
  public virtual Currency? Currency { get; private set; }

  public TimeSpan? Duration { get; private set; } // Nullable for once services or subscription services with no duration or end date
  public DateTime PublishedAt { get; private set; }
  public bool IsOtherActivate { get; private set; } // Активируется вне платформы

  public IReadOnlyCollection<Category> Categories => _catgories.AsReadOnly();
  public IReadOnlyCollection<ProvideService> ProvideServices => _provideServices.AsReadOnly();
}

public enum ServiceType
{
  Once,
  Subscription,
}
