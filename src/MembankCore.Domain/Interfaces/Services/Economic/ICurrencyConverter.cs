using MembankCore.Domain.Entities.Economic;
using MembankCore.Domain.ValueObjects;

namespace MembankCore.Domain.Interfaces.Services.Economic;

public interface ICurrencyConverter {
  /// <summary>
  /// Конвертирует денежную сумму из одной валюты в другую.
  /// </summary>
  /// <param name="amount">Исходная сумма и её валюта</param>
  /// <param name="sourceCurrency">Объект исходной валюты (с курсом)</param>
  /// <param name="targetCurrency">Объект целевой валюты (с курсом)</param>
  /// <returns>Новый объект Money в целевой валюте</returns>
  Money Convert(Money amount, Currency sourceCurrency, Currency targetCurrency);
}
