using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MembankCore.Domain.ValueObjects;

namespace MembankCore.Domain.Entities.Economic;

public enum TransferNoteType {
  Personal,
  Buying,
  Tax,
  Emission,
  Subscription
}

public class TransferNote {
  public Guid Id { get; private set; }
  public TransferNoteType Type { get; private set; }

  public Guid SenderId { get; private set; }
  public virtual Wallet Sender { get; private set; }

  public Guid ReceiverId { get; private set; }
  public virtual Wallet Receiver { get; private set; }

  public Money Amount { get; private set; }

  public string CurrencyId => Amount.CurrencyId;
  public virtual Currency Currency { get; private set; }

  public decimal ExchangeRateAtTransfer { get; private set; }

  public DateTime NotedAt { get; private set; }
  public string? Description { get; private set; }

  protected TransferNote() { }

  public TransferNote(
    Wallet sender,
    Wallet receiver,
    Currency currency,
    Money amount,
    TransferNoteType type,
    string? description = null) {
    Id = Guid.NewGuid();

    SenderId = sender.Id;
    Sender = sender;

    ReceiverId = receiver.Id;
    Receiver = receiver;

    if (currency.Id != amount.CurrencyId)
      throw new Exception("Валюта суммы не совпадает с объектом валюты.");

    Amount = amount;

    ExchangeRateAtTransfer = currency.ExchangeRate;

    Type = type;
    Description = description;
    NotedAt = DateTime.UtcNow;
  }
}
