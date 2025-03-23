using Core.Db.Accounts;
using Core.Db.Users;
namespace Core.Db
{
  public static class DbUtils
  {
    public static void Init()
    {
      using DBContext db = new DBContext();
      if(!db.Database.CanConnect())
      {
        db.Database.EnsureCreated();

        db.SaveChanges();
        db.Users.Add(new User
            {
            Name = "admin",
            Email = "admin@admin.com",
            Password = "admin",
            IsOver18 = true,
            Role = Role.Admin
            });
        db.Users.Add(new User
            {
            Name = "Bob Smith USER",
            Email = "bob@bob.com",
            Password = "bob",
            Role = Role.User,
            IsOver18 = true,
            });

        db.Users.Add(new User
            {
            Name = "Bob Smith MANAGER",
            Email = "alice@alice.com",
            Password = "alice",
            Role = Role.Manager,
            IsOver18 = true,
            });
        db.SaveChanges();
        db.Accounts.Add(new Account
            {
            Name = "Checking",
            Type = AccountType.Checking,
            WeeklyLimit = 1000,
            Balance = 1000,
            OwnerId = db.Users.First(u => u.Email == "admin@admin.com").Id
            });
        db.Accounts.Add(new Account
            {
            Name = "Savings",
            Type = AccountType.Savings,
            WeeklyLimit = 1000,
            Balance = 0,
            OwnerId = db.Users.First(u => u.Email == "bob@bob.com").Id
            });
        db.Accounts.Add(new Account
            {
            Name = "Credit",
            Type = AccountType.Credit,
            WeeklyLimit = 1000,
            Balance = 0,
            OwnerId = db.Users.First(u => u.Email == "bob@bob.com").Id
            });
        db.SaveChanges();
        AccountOperations checkingAccountOperations = new CheckingAccountOperations(db.Users.First(u => u.Email == "admin@admin.com"), db.Accounts.First(a => a.Type == AccountType.Checking), db);
        checkingAccountOperations.SendMoney(db.Accounts.First(a => a.Type == AccountType.Savings).Id, 10);
        checkingAccountOperations.SendMoney(db.Accounts.First(a => a.Type == AccountType.Credit).Id, 10);


      }
      
      db.SaveChanges();
    }
  }
  public class Operations
  {
    public Dictionary<Guid,AccountOperations> accountOperations { get; set; }
    public UserOpetaions userOpetaions { get; set; }
    private AccountOperations decideAccountOperations(AccountType type, User _user, Account _account, DBContext _db)
    {
      switch (type)
      {
        case AccountType.Credit:
          return new CreditAccountOperations(_user, _account, _db);
        case AccountType.Savings:
          return new SavingsAccountOperations(_user, _account, _db);
        case AccountType.Checking:
          return new CheckingAccountOperations(_user, _account, _db);
        default:
          throw new ArgumentException("Invalid account type provided");
      }
    }
    public Operations(Role role, User _user, Account _account, DBContext _db)
    {
      switch (role)
      {
        case Role.Admin:
          userOpetaions = new AdminUserOperations(_user, _db);

          break;
        case Role.Manager:
          userOpetaions = new ManagerUserOperations(_user, _db);
          break;
        case Role.User:
          userOpetaions = new UserUserOperations(_user, _db);
          break;
      }

      foreach (var account in _db.Accounts)
      {
        accountOperations.Add(account.Id, decideAccountOperations(account.Type, _user, account, _db));
      }
    }
  }
  public class Ctx
  {
    public DBContext db = new DBContext();
    public User User { get; set; }
    public Account Account { get; set; }
    public Operations operations { get; set; }
    public Ctx(User _user, Account _account, DBContext _db)
    {
      User= _user;
      operations = new Operations(Role.User, _user, _account, _db);

    }
  }



  public readonly struct Result<T, E> {
    private readonly bool _success;
    public readonly T Value;
    public readonly E Error;

    private Result(T v, E e, bool success)
    {
      Value = v;
      Error = e;
      _success = success;
    }

    public bool IsOk => _success;

    public static Result<T, E> Ok(T v)
    {
      return new(v, default(E), true);
    }

    public static Result<T, E> Err(E e)
    {
      return new(default(T), e, false);
    }

    public static implicit operator Result<T, E>(T v) => new(v, default(E), true);
    public static implicit operator Result<T, E>(E e) => new(default(T), e, false);

    public R Match<R>(
        Func<T, R> success,
        Func<E, R> failure) =>
      _success ? success(Value) : failure(Error);
  }

  public class AuthRequest
  {
    public string Email { get; set; }
    public string Password { get; set; }
  }
}


