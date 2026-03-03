using MembankCore.Domain.Entities.Economic;
using MembankCore.Domain.ValueObjects;

namespace MembankCore.Domain.Interfaces.Services.Economic;

/// <summary>
/// Результат перевода денег, нужен для заполнения отчёта об транзакции
/// </summary>
/// <param name="SentAmount">Сколько ушло в валюте отправителя (с комиссией)</param>
/// <param name="ReceivedAmount">Сколько пришло в валюте получателя</param>
/// <param name="Fee">Комиссия в исходной валюте перевода</param>
/// <param name="RateAtTransfer">Курс на момент операции</param>
/// <seealso cref="TransferNote"/>
public record TransferResult(
  Money SentAmount,
  Money ReceivedAmount,
  Money Fee,
  decimal RateAtTransfer
);

public interface ITransferProcessor {
  /// <summary>
  /// Выполняет перевод денег суммой <paramref name="amount"/> из кошелька <paramref name="sender"/>
  /// в кошелёк <paramref name="receiver"/>
  /// </summary>
  /// <param name="sender">Кошелёк <see cref="Wallet"/> отправителя</param>
  /// <param name="receiver">Кошелёк <see cref="Wallet"/> получателя</param>
  /// <param name="amount">Количество средств в валюте, определенной <see cref="Money"/></param>
  /// <param name="hasPrime">Есть ли у пользователя прайм статус?</param>
  /// <param name="type">Объект <see  cref="TransferNoteType"/>, содержащий тип транзакции</param>
  /// <returns>Объект <see cref="TransferResult"/> с результатом перевода</returns>
  TransferResult Execute(
    Wallet sender,
    Wallet receiver,
    Money amount,
    bool hasPrime,
    TransferNoteType type);
}
