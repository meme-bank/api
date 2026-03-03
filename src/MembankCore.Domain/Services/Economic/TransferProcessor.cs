using MembankCore.Domain.ValueObjects;
using MembankCore.Domain.Entities.Economic;
using MembankCore.Domain.Services.Economic;

namespace MembankCore.Domain.Services.Economic;

public record TransferResult(
    Money SentAmount,      // Сколько ушло в валюте отправителя (с комиссией)
    Money ReceivedAmount,  // Сколько пришло в валюте получателя
    Money Fee,             // Комиссия в исходной валюте перевода
    decimal RateAtTransfer // Курс на момент операции
);

public class TransferProcessor
{
  public TransferResult Execute(
    Wallet sender,
    Wallet receiver,
    Money amount,
    bool hasPrime,
    TransferNoteType type,
    CurrencyConverter converter
  )
  {
    var feePercentage = (hasPrime || type != TransferNoteType.Personal) ? 0m : 0.05m;
    var fee = new Money(amount.Amount * feePercentage, amount.CurrencyId);

    var totalToSpend = amount + fee;

    var senderDebit = converter.Convert(totalToSpend, sender.Currency, sender.Currency);
    var receiverCredit = converter.Convert(amount, sender.Currency, receiver.Currency);

    sender.Withdraw(senderDebit);
    receiver.Deposit(receiverCredit);

    return new TransferResult(senderDebit, receiverCredit, fee, receiver.Currency.ExchangeRate);
  }
}
