using MembankCore.Domain.Entities.Core;

namespace MembankCore.Domain.Entities.Economic;

/// <summary>
/// Валюта, то есть изменчивое отношение (курса) к деньгам в системе
/// </summary>
/// <seealso cref="CurrencyRateHistory"/>
public class Currency {
  private readonly List<int> _legalTenderCountryIds = [];
  private readonly List<CurrencyRateHistory> _rateHistory = [];

  public string Id { get; private set; }
  public string Name { get; private set; }

  public decimal ExchangeRate { get; private set; }
  public decimal TotalSupply { get; private set; } // сколько выпущенно

  public Guid MonochromePhotoId { get; private set; }
  public virtual Photo MonochromePhoto { get; private set; } = null!; // symbol

  public int IssuerId { get; private set; } // country that emits this currency
  public DateTime CreatedAt { get; private set; }
  public DateTime LastRateUpdate { get; private set; }

  public IReadOnlyCollection<int> LegalTenderCountryIds => _legalTenderCountryIds.AsReadOnly();
  public IReadOnlyCollection<CurrencyRateHistory> RateHistory => _rateHistory.AsReadOnly();


  public Currency(string code, string name, decimal rate, int issuerId, Photo symbol) {
    if (string.IsNullOrWhiteSpace(code) || code.Length != 3)
      throw new Exception("Код валюты должен состоять из 3 символов (ISO).");

    Id = code.ToUpper();
    Name = name;
    MonochromePhoto = symbol;
    MonochromePhotoId = symbol.Id;
    ExchangeRate = rate;
    IssuerId = issuerId;
    CreatedAt = DateTime.UtcNow;
  }

  /// <summary>
  /// Добавить страну, где это является легальным средством платежей
  /// </summary>
  /// <param name="id">Идентификатор страны</param>
  public void AddLegalTenderCountryId(int id) {
    _legalTenderCountryIds.Add(id);
  }

  /// <summary>
  /// Сменить аккаунт, ответственный за выпуск валюты
  /// </summary>
  /// <param name="id">Идентификатор аккаунта, ответственного за выпус валютык</param>
  public void ChangeIssuerId(int id) {
    IssuerId = id;
    _legalTenderCountryIds.Remove(id);
  }

  /// <summary>
  /// Обновить курс валюты (отношение к основной валюте)
  /// </summary>
  /// <param name="newRate">Новое отношение к основной валюте (сколько стоит основная валюта в данной валюте)</param>
  /// <exception cref="ArgumentException">Исключение, связанное с установкой неположительного курса</exception>
  public void UpdateExchangeRate(decimal newRate) {
    if (newRate <= 0)
      throw new ArgumentException("Курс валюты должен быть положительным.");

    var now = DateTime.UtcNow;
    ExchangeRate = newRate;
    LastRateUpdate = now;
    _rateHistory.Add(new CurrencyRateHistory(this, newRate, now));
  }

  /// <summary>
  /// Эмиссия валюты
  /// </summary>
  /// <remarks>Не использовать вне сервиса</remarks>
  /// <param name="amount">Сумма эмиссии</param>
  /// <exception cref="ArgumentException">Исключение, связанное с эмиссией неположительной суммы</exception>
  public void Mint(decimal amount) {
    if (amount <= 0) throw new ArgumentException("Сумма эмиссии должна быть положительной.");
    TotalSupply += amount;
  }

  /// <summary>
  /// Изъятие валюты
  /// </summary>
  /// <remarks>Не использовать вне сервиса</remarks>
  /// <param name="amount">Сумма изъятия</param>
  /// <exception cref="ArgumentException">Исключение, связанное с изъятием неположительной суммы</exception>
  public void Burn(decimal amount) {
    if (amount <= 0) throw new ArgumentException("Сумма изъятия должна быть положительной.");
    if (TotalSupply < amount) throw new InvalidOperationException("Недостаточно массы для изъятия.");
    TotalSupply -= amount;
  }
}
