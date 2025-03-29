using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace OctopusAPI.Database
{
  public class MeduzaContext : DbContext
  {
    // -- Database Connection -- //
    private string User = "meduza";
    private string DatabaseName = "meduza";
    private string Host = "127.0.0.1";
    private string Password = "meduza";
    private int Port = 5432;

    // -- Entities -- //
    // Core Entities //
    public DbSet<Entities.Core.Photo> Photos { get; set; }
    public DbSet<Entities.Core.Setting> Settings { get; set; }

    // Economic Base Entities //
    public DbSet<Entities.Economic.Wallet> Wallets { get; set; }
    public DbSet<Entities.Economic.TransferNote> TransferNotes { get; set; }
    public DbSet<Entities.Economic.Currency> Currencies { get; set; }

    // Trading Entities //
    // Material Items
    public DbSet<Entities.Trading.Item> Items { get; set; }
    public DbSet<Entities.Trading.ItemBlueprint> ItemBlueprints { get; set; }
    public DbSet<Entities.Trading.Product> Products { get; set; }

    // Services
    public DbSet<Entities.Trading.Service> Services { get; set; }
    public DbSet<Entities.Trading.ProvideService> ProvideServices { get; set; }

    // Categories of Services, Tariffs and Items
    public DbSet<Entities.Trading.Category> Categories { get; set; }

    // Blog Entities //
    public DbSet<Entities.Blog.Post> Posts { get; set; }
    public DbSet<Entities.Blog.Blog> Blogs { get; set; }

    // -- Constructor -- //
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    => optionsBuilder
          .UseNpgsql($"Server={Host};Port={Port};Database={DatabaseName};User Id={User};Password={Password}")
          .UseSnakeCaseNamingConvention();
  }
}