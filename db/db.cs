using Microsoft.EntityFrameworkCore;

namespace Core.Db
{

  public enum Role
  {
    Admin,
    Manager,
    User,
  }
  public enum AccountType
  {
    Checking,
    Savings,
    Credit,
  }
  public class User
  {
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public Role Role { get; set; }
    public bool IsOver18 { get; set; }
    public ICollection<Account> Accounts { get; } = new List<Account>();
  }

  public class  Account
  {
    public AccountType Type { get; set; }
    public string Name { get; set; }
    public Guid Id { get; set; }
    public Guid OwnerId { get; set; } 
    public User Owner { get; set; } = null!;
    public int Balance { get; set; }
    public ICollection<Transaction> TransactionsRecived { get; } = new List<Transaction>();
    public ICollection<Transaction> TransactionsSent { get; } = new List<Transaction>();
  }
  public class Transaction
  {
    public Guid Id { get; set; }
    public Account Sender { get; set; } = null!;
    public Account Reciver { get; set; } = null!;
    public Guid SenderId { get; set; }
    public Guid ReciverId { get; set; }
    public int Amount { get; set; }
  }

  public class DBContext : DbContext
  {
    public DbSet<User> Users { get; set; }
    public DbSet<Account> Accounts { get; set; }
    public DbSet<Transaction> Transactions { get; set; }
    public string DbPath { get; }

    public DBContext()
    {
        var folder = Environment.SpecialFolder.LocalApplicationData;
        string path = Environment.GetFolderPath(folder);
        DbPath = System.IO.Path.Join(path, "bank.db");
        Console.WriteLine(DbPath);
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      modelBuilder.Entity<User>()
        .HasMany(e => e.Accounts)
        .WithOne(e => e.Owner)
        .HasForeignKey(e => e.OwnerId)
        .IsRequired();
      modelBuilder.Entity<Account>()
        .HasMany(e => e.TransactionsRecived)
        .WithOne(e => e.Reciver)
        .HasForeignKey(e => e.ReciverId)
        .IsRequired();
      modelBuilder.Entity<Account>()
        .HasMany(e => e.TransactionsSent)
        .WithOne(e => e.Sender)
        .HasForeignKey(e => e.SenderId)
        .IsRequired();
      
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite($"Data Source={DbPath}");
  }

}
