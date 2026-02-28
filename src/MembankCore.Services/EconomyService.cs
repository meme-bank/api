using MembankCore.Data.Base;
using MembankCore.Domain.Entities.Core;
using MembankCore.Domain.Entities.Economic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace MembankCore.Services {
  public class EconomyService {
    private readonly MeduzaContext _context;

    private readonly Guid bankWalletId = Guid.Parse("00000000-0000-0000-0000-000000000001");
    private readonly Guid primeServiceId = Guid.Parse("00000000-0000-0000-0000-00000000A001");

    public EconomyService(MeduzaContext context) {
      _context = context;
    }

    public async Task<decimal> ConvertCurrencyAsync(decimal amount, string fromCurrencyId, string toCurrencyId) {
      var fromCurrency = await _context.Currencies.FindAsync(fromCurrencyId);
      var toCurrency = await _context.Currencies.FindAsync(toCurrencyId);

      if (fromCurrency == null || toCurrency == null) {
        throw new ArgumentException("Invalid currency ID");
      }

      if (toCurrency.ExchangeRate <= 0)
        throw new DivideByZeroException($"Курс валюты {toCurrencyId} установлен некорректно (ноль или меньше).");

      decimal amountInBase = amount * fromCurrency.ExchangeRate;
      decimal convertedAmount = amountInBase / toCurrency.ExchangeRate;

      return convertedAmount;
    }

    public async Task<bool> HasPrimeAsync(int buyerId) => await _context.ProvideServices
        .AnyAsync(ps => ps.BuyerId == buyerId &&
        ps.ServiceId == primeServiceId &&
        (ps.ExpiresAt == null || ps.ExpiresAt > DateTime.UtcNow));

    public async Task<TransferNote> TransferAsync(Guid senderId, Guid receiverId, decimal amount, string currencyId, string? note, IDbContextTransaction? transaction, TransferNoteType type = TransferNoteType.Personal) {
      if (transaction == null) {
        using var newTransaction = await _context.Database.BeginTransactionAsync();
        try {
          var result = await TransferAsync(senderId, receiverId, amount, currencyId, note, newTransaction, type);
          await _context.SaveChangesAsync();
          await newTransaction.CommitAsync();
          return result;
        }
        catch {
          await newTransaction.RollbackAsync();
          throw;
        }
      }
      var sender = await _context.Wallets.FindAsync(senderId);
      var receiver = await _context.Wallets.FindAsync(receiverId);
      Currency? currency = await _context.Currencies.FindAsync(currencyId);

      if (sender == null || receiver == null || currency == null)
        throw new ArgumentException("Недействительный идентификатор кошелька/ов или валюты");

      // Проверка на наличие Prime и расчет комиссии
      var hasPrime = await HasPrimeAsync(sender.OwnerId);

      var feePercentage = hasPrime || type != TransferNoteType.Personal ? 0m : 0.05m; // 5% комиссия без Prime для персональных переводов

      // Вычисления комиссии и итоговых сум
      decimal feeAmount = amount * feePercentage;

      if (feeAmount > 0) {
        Wallet? bankWallet = await _context.Wallets.FindAsync(bankWalletId);
        if (bankWallet == null)
          feeAmount = 0;
        bankWallet?.Deposit(await ConvertCurrencyAsync(feeAmount, currencyId, bankWallet.CurrencyId));
      }

      decimal totalToSpend = amount + feeAmount;

      decimal amountInSenderCurrency = await ConvertCurrencyAsync(totalToSpend, currencyId, sender.CurrencyId);
      decimal amountRecive = await ConvertCurrencyAsync(amount, currencyId, receiver.CurrencyId);

      sender.Withdraw(amountInSenderCurrency);
      receiver.Deposit(amountRecive);

      var transferNote = new TransferNote() {
        Id = Guid.NewGuid(),
        Sender = sender,
        Receiver = receiver,
        Amount = amount,
        SenderId = senderId,
        ReceiverId = receiverId,
        NotedAt = DateTime.UtcNow,
        Currency = currency,
        CurrencyId = currency.Id,
        ExchangeRateAtTransfer = currency.ExchangeRate,
        Description = note,
        Type = type,
      };

      _context.TransferNotes.Add(transferNote);
      return transferNote;
    }

    public async Task<Wallet> CreateWalletAsync(int ownerId, string currencyId, string name) {
      Wallet wallet = new() {
        Id = Guid.NewGuid(),
        OwnerId = ownerId,
        CurrencyId = currencyId,
        Balance = 0m,
        CreatedAt = DateTime.UtcNow,
        Name = name,
        Currency = await _context.Currencies.FindAsync(currencyId) ?? throw new ArgumentException("Invalid currency ID")
      };

      _context.Wallets.Add(wallet);
      await _context.SaveChangesAsync();

      return wallet;

    }
    public async Task<Currency> CreateCurrencyAsync(string id, string name, int emmissionCountryId) {
      Currency currency = new() {
        Id = id,
        Name = name,
        EmmissionCountryId = emmissionCountryId,
        MonochromePhoto = new Photo {
          Id = Guid.NewGuid(),
          Type = PhotoType.Icon,
          RequestedAt = DateTime.UtcNow,
          OwnerId = emmissionCountryId
        },
        CreatedAt = DateTime.UtcNow
      };

      _context.Currencies.Add(currency);
      await _context.SaveChangesAsync();

      return currency;
    }
  }
}
