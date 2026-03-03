using MembankCore.Domain.Entities.Core;

namespace MembankCore.Domain.Entities.Economic;

public class Currency {
  private readonly List<int> _legalTenderCountryIds = [];
  private readonly List<CurrencyRateHistory> _rateHistory = [];

  public string Id { get; private set; }
  public string Name { get; private set; }

  public decimal ExchangeRate { get; private set; } = 100; // against Leuro or another base currency (BC - Base Currency, AC - Alternative Currency, 1AC = 100BC => 0.01, 100AC = 1BC => 1.0)
  public decimal TotalSupply { get; private set; } // сколько выпущенно

  public Guid MonochromePhotoId { get; private set; }
  public virtual Photo MonochromePhoto { get; private set; } = null!; // symbol

  public int IssuerId { get; private set; } // country that emits this currency
  public DateTime CreatedAt { get; private set; }
  public DateTime LastRateUpdate { get; private set; }

  public IReadOnlyCollection<int> LegalTenderCountryIds => _legalTenderCountryIds.AsReadOnly();
  public IReadOnlyCollection<CurrencyRateHistory> RateHistory => _rateHistory.AsReadOnly();


  public Currency(string code, string name, decimal rate, Photo symbol) {
    if (string.IsNullOrWhiteSpace(code) || code.Length != 3)
      throw new Exception("Код валюты должен состоять из 3 символов (ISO).");

    Id = code.ToUpper();
    Name = name;
    MonochromePhoto = symbol;
    MonochromePhotoId = symbol.Id;
    ExchangeRate = rate;
    CreatedAt = DateTime.UtcNow;
  }

  public void UpdateExchangeRate(decimal newRate) {
    if (newRate <= 0)
      throw new Exception("Курс валюты должен быть положительным.");

    var now = DateTime.UtcNow;
    ExchangeRate = newRate;
    LastRateUpdate = now;
    _rateHistory.Add(new CurrencyRateHistory(this, newRate, now));
  }

  public void Mint(decimal amount) {
    if (amount <= 0) throw new ArgumentException("Сумма эмиссии должна быть положительной.");
    TotalSupply += amount;
  }

  public void Burn(decimal amount) {
    if (amount <= 0) throw new ArgumentException("Сумма изъятия должна быть положительной.");
    if (TotalSupply < amount) throw new InvalidOperationException("Недостаточно массы для изъятия.");
    TotalSupply -= amount;
  }
}
