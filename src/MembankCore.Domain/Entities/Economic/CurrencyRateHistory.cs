namespace MembankCore.Domain.Entities.Economic;

public class CurrencyRateHistory {
  public Guid Id { get; private set; } = Guid.NewGuid();
  public string CurrencyId { get; private set; }
  public virtual Currency Currency { get; private set; }
  public decimal Rate { get; private set; }
  public DateTime ValidFrom { get; private set; }

  public CurrencyRateHistory(Currency currency, decimal rate, DateTime validFrom) {
    Currency = currency;
    CurrencyId = currency.Id;
    Rate = rate;
    ValidFrom = validFrom;
  }
}
