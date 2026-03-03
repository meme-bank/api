using MembankCore.Domain.Entities.Economic;
using MembankCore.Domain.Interfaces.Services.Economic;
using MembankCore.Domain.Services.Economic;
using MembankCore.Domain.ValueObjects;

namespace MembankCore.Domain.Services.Economic;

public class TransferProcessor : ITransferProcessor {
  private readonly ICurrencyConverter _converter;

  public TransferProcessor(ICurrencyConverter converter) {
    _converter = converter;
  }

  public TransferResult Execute(
    Wallet sender,
    Wallet receiver,
    Money amount,
    bool hasPrime,
    TransferNoteType type
  ) {
    var feePercentage = (hasPrime || type != TransferNoteType.Personal) ? 0m : 0.05m;
    var fee = new Money(amount.Amount * feePercentage, amount.CurrencyId);

    var totalToSpend = amount + fee;

    var senderDebit = _converter.Convert(totalToSpend, sender.Currency, sender.Currency);
    var receiverCredit = _converter.Convert(amount, sender.Currency, receiver.Currency);

    sender.Withdraw(senderDebit);
    receiver.Deposit(receiverCredit);

    return new TransferResult(senderDebit, receiverCredit, fee, receiver.Currency.ExchangeRate);
  }
}
