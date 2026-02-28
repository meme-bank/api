using MembankCore.Domain.Entities.Blog;
using MembankCore.Domain.Entities.Core;
using MembankCore.Domain.Entities.Economic;
using MembankCore.Domain.Entities.Loan;
using MembankCore.Domain.Entities.Stocks;
using MembankCore.Domain.Entities.Trading;
using Microsoft.EntityFrameworkCore;

namespace MembankCore.Data.Base {
  public class MeduzaContext : DbContext {
    // -- Database Connection -- //
    private string User = "meduza";
    private string DatabaseName = "meduza";
    private string Host = "127.0.0.1";
    private string Password = "meduza";
    private int Port = 5432;

    // -- Entities -- //
    // Core Entities //
    public DbSet<Photo> Photos { get; set; }
    public DbSet<Setting> Settings { get; set; }

    // Economic Base Entities //
    public DbSet<Wallet> Wallets { get; set; }
    public DbSet<TransferNote> TransferNotes { get; set; }
    public DbSet<Currency> Currencies { get; set; }

    // Trading Entities //
    // Material Items
    public DbSet<Item> Items { get; set; }
    public DbSet<ItemBlueprint> ItemBlueprints { get; set; }
    public DbSet<Product> Products { get; set; }

    // Services
    public DbSet<Service> Services { get; set; }
    public DbSet<ProvideService> ProvideServices { get; set; }

    // Categories of Services, Tariffs and Items
    public DbSet<Category> Categories { get; set; }

    // Blog Entities //
    public DbSet<Post> Posts { get; set; }
    public DbSet<Blog> Blogs { get; set; }

    // Loan Entities //
    public DbSet<Loan> Loans { get; set; }
    public DbSet<Deposit> Deposits { get; set; }

    // -- Constructor -- //
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    => optionsBuilder
                .UseNpgsql($"Server={Host};Port={Port};Database={DatabaseName};User Id={User};Password={Password}")
                .UseSnakeCaseNamingConvention();

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
      base.OnModelCreating(modelBuilder);
      SetupRelationships(modelBuilder);
      SetupProperties(modelBuilder);
      SeedInitialData(modelBuilder);
    }

    private void SetupProperties(ModelBuilder modelBuilder) {
      foreach (var property in modelBuilder.Model.GetEntityTypes()
      .SelectMany(t => t.GetProperties())
      .Where(p => p.ClrType == typeof(decimal))) {
        property.SetPrecision(18);
        property.SetScale(4);
      }
    }

    private void SetupRelationships(ModelBuilder modelBuilder) {
      // --- Many-to-many ---
      // Wallet <-> TransferNote
      modelBuilder.Entity<Wallet>()
              .HasMany(w => w.RecivedTransferNotes)
              .WithOne(tn => tn.Receiver)
              .HasForeignKey(tn => tn.ReceiverId)
              .OnDelete(DeleteBehavior.Restrict);

      modelBuilder.Entity<Wallet>()
              .HasMany(w => w.SentTransferNotes)
              .WithOne(tn => tn.Sender)
              .HasForeignKey(tn => tn.SenderId)
              .OnDelete(DeleteBehavior.Restrict);

      // Service, Items <-> Category
      modelBuilder.Entity<Service>()
              .HasMany(s => s.Categories)
              .WithMany(c => c.Services);

      modelBuilder.Entity<ItemBlueprint>()
              .HasMany(p => p.Categories)
              .WithMany(c => c.ItemBlueprints);

      // ItemBlueprint <-> ItemBlueprint (Recipe)
      modelBuilder.Entity<ItemBlueprintRecipe>(entity => {
        // Составной ключ, чтобы один и тот же ингредиент не дублировался в одном рецепте
        entity.HasKey(r => new { r.RecipeItemId, r.CraftItemId });

        entity.HasOne(r => r.RecipeItem)
                            .WithMany(b => b.CraftableItems) // Где этот предмет используется как сырье
                            .HasForeignKey(r => r.RecipeItemId);

        entity.HasOne(r => r.CraftItem)
                            .WithMany(b => b.Recipe) // Из чего состоит этот предмет
                            .HasForeignKey(r => r.CraftItemId);
      });
    }

    private void SeedInitialData(ModelBuilder modelBuilder) {
      var leuroCurrencyId = "LMC";
      var bankId = 3; // НБМ
      var fallelandId = 2; // Ловушкинск
      var adminId = 1; // Артемос, то бишь я
      var bankWalletId = Guid.Parse("00000000-0000-0000-0000-000000000001");
      var fallelandWalletId = Guid.Parse("00000000-0000-0000-0000-000000000002");
      var adminWalletId = Guid.Parse("00000000-0000-0000-0000-000000000123");
      var defaultPhotoId = "default_currency_photo";

      modelBuilder.Entity<Photo>().HasData(new {
        Id = defaultPhotoId,
        OwnerId = bankId,
        Image = new byte[0], // Пустой массив для инициализации
        RequestedAt = DateTime.UtcNow,
        UploadedAt = DateTime.UtcNow,
        Type = PhotoType.Icon // Или другой подходящий тип
      });

      modelBuilder.Entity<Currency>().HasData(new {
        Id = leuroCurrencyId,
        Name = "Левро",
        ExchangeRate = 1.0m, // Эталон
        EmmissionCountryId = fallelandId, // Ловушкинск
        CreatedAt = new DateTime(2020, 8, 25, 0, 0, 0, DateTimeKind.Utc),
        MonochromePhotoId = "default_currency_photo"
      });

      modelBuilder.Entity<Wallet>().HasData(new {
        Id = bankWalletId,
        Name = "Резерв НБМ",
        Description = "Главный кошелек для сбора банковских комиссий и эмиссии",
        CurrencyId = leuroCurrencyId,
        OwnerId = bankId,
        Balance = 1000000000m, // Начальный капитал банка
        CreatedAt = new DateTime(2020, 8, 25, 0, 0, 0, DateTimeKind.Utc)
      });

      modelBuilder.Entity<Wallet>().HasData(new {
        Id = fallelandWalletId,
        Name = "Резерв Ловушкинска",
        Description = "Главный кошелек для сбора налогов",
        CurrencyId = leuroCurrencyId,
        OwnerId = fallelandId,
        Balance = 1_000_000_000_000m,
        CreatedAt = new DateTime(2020, 8, 25, 0, 0, 0, DateTimeKind.Utc)
      });

      modelBuilder.Entity<Wallet>().HasData(new {
        Id = adminWalletId,
        Name = "Личный кошелек",
        Description = "Личный кошелек администратора системы",
        CurrencyId = leuroCurrencyId,
        OwnerId = adminId,
        Balance = 1_000_000_000_000m,
        CreatedAt = new DateTime(2020, 8, 25, 0, 0, 0, DateTimeKind.Utc)
      });

      modelBuilder.Entity<Category>().HasData(new {
        Id = Guid.Parse("00000000-0000-0000-0000-000000000002"),
        Name = "Банкинг",
        Description = "Услуги, связанные с банковскими операциями и финансовыми сервисами."
      });

      modelBuilder.Entity<Service>().HasData(new {
        CurrencyId = leuroCurrencyId,
        Id = Guid.Parse("00000000-0000-0000-0000-00000000A001"),
        Name = "НБМ Prime",
        Description = "Премиум подписка на эксклюзивные банковские услуги от Народного Банка Мемов.",
        ProviderId = bankId,
        PublishedAt = DateTime.UtcNow,
        Type = ServiceType.Subscription,
        Duration = TimeSpan.FromDays(30),
        IsOtherActivate = true
      });

      modelBuilder.Entity<Service>()
              .HasMany(s => s.Categories)
              .WithMany(c => c.Services)
              .UsingEntity(j => j.HasData(new {
                ServicesId = Guid.Parse("00000000-0000-0000-0000-00000000A001"),
                CategoriesId = Guid.Parse("00000000-0000-0000-0000-000000000002")
              }));
    }
  }
}
