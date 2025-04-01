namespace Core.Db.Users
{
  public class UserOpetaions
  {
    private User ctx { get; set; }
    private DBContext db { get; set; }

    public UserOpetaions(User _ctx, DBContext _db)
    {
      ctx = _ctx;
      db = _db;
    }
    public Result<User, string> CreateUser(string name,bool isOver18, string email, string password, Role role)
    {
      try
      {
        User user = new User
        {
          Name = name,
               Email = email,
               Password = password,
               IsOver18 = isOver18,
               Role = role
        };
        db.Users.Add(user);
        db.SaveChanges();
        return  user;
      }
      catch (System.Exception e)
      {
        Console.WriteLine(e.Message);
        return "500";
      }
    }
    public virtual Guid GetATMID()
    {
      Guid id = db.Users.Where(acc=>acc.Email == "cash@bank.com").First().Id;

      return db.Accounts.Where(acc=>acc.OwnerId == id).First().Id;
    }
    public virtual Result<Account, string> CreateAccount(string name,AccountType accountType,User user)
    {
      try
      {
        Account account = new Account
        {
          Name = name,
               Type = accountType,
               Owner = user,
               Id = Guid.NewGuid(),
               Balance = 0,
               OwnerId = user.Id,
        };
        db.Accounts.Add(account);
        db.SaveChanges();
        return  account;
      }
      catch (System.Exception e)
      {
        Console.WriteLine(e.Message);
        return "500";
      }
    }
    public virtual Result<Account, string> CreateAccount(string name,AccountType accountType)
    {
      try
      {
        Account account = new Account
        {
          Name = name,
               Type = accountType,
               Owner = ctx,
               Id = Guid.NewGuid(),
               Balance = 0,
               OwnerId = ctx.Id,
        };
        db.Accounts.Add(account);
        db.SaveChanges();
        return  account;
      }
      catch (System.Exception e)
      {
        Console.WriteLine(e.Message);
        return "500";
      }
    }
    public virtual void ClockIntrest()
    {
      new Exception("Permission denied");
    }

    public virtual Account[] GetAccounts()
    {
      throw new System.NotImplementedException();
    }
    public virtual Transaction[] GetTransactions()
    {
      throw new System.NotImplementedException();
    }
    public virtual User[] GetUsers()
    {
      throw new Exception("Permission denied");
    }
    public virtual void DeleteUser(Guid id)
    {
      throw new Exception("Permission denied");
    }

  }
  public class AdminUserOperations : UserOpetaions
  {

    private User ctx { get; set; }
    private DBContext db { get; set; }

    public AdminUserOperations(User _ctx, DBContext _db) : base(_ctx, _db)
    {
      ctx = _ctx;
      db = _db;
    }

    public new Result<User, string> CreateUser(string name,bool isOver18, string email, string password, Role role)
    {
      return base.CreateUser(name, isOver18, email, password, role);
    }
    public override Account[] GetAccounts()
    {
      return db.Accounts.ToArray();
    }
    public override Transaction[] GetTransactions()
    {
      return db.Transactions.ToArray();
    }
    public override  Result<Account, string> CreateAccount(string name,AccountType accountType)
    {
      return base.CreateAccount(name, accountType);
    }
    public override  Result<Account, string> CreateAccount(string name,AccountType accountType, User user)
    {
      return base.CreateAccount(name, accountType, user);
    }
    public override User[] GetUsers()
    {
      return db.Users.ToArray();
    }
    public override void DeleteUser(Guid id)
    {
      db.Users.Remove(db.Users.First(e => e.Id == id));
      db.SaveChanges();
    }

  }
  public class ManagerUserOperations : UserOpetaions
  {

    private User ctx { get; set; }
    private DBContext db { get; set; }

    public ManagerUserOperations(User _ctx, DBContext _db) : base(_ctx, _db)
    {
      ctx = _ctx;
      db = _db;
    }
    public Result<User, string> CreateUser(string name,int age, string email, string password, Role role) => "Only admin can create users";
    public override Account[] GetAccounts()
    {
      return db.Accounts.ToArray();
    }
    public override Transaction[] GetTransactions()
    {
      return db.Transactions.Take(100).ToArray();
    }
    public override  Result<Account, string> CreateAccount(string name,AccountType accountType)
    {
      return base.CreateAccount(name, accountType);
    }
    public override  Result<Account, string> CreateAccount(string name,AccountType accountType, User user)
    {
      return "Not Authorized";
    }
    public override void ClockIntrest()
    {
      foreach (var account in db.Accounts.Where(a => a.Type == AccountType.Savings))
      {
        account.Balance += (int)(account.Balance * 0.02m);
      }
      foreach (var account in db.Accounts.Where(a => a.Type == AccountType.Credit && a.Balance < 0))
      {
        account.Balance += (int)(account.Balance * 0.05m);
      }

      db.SaveChanges();
    }

  }
  public class UserUserOperations : UserOpetaions
  {
    private User ctx { get; set; }
    private DBContext db { get; set; }

    public UserUserOperations(User _ctx, DBContext _db) : base(_ctx, _db)
    {
      ctx = _ctx;
      db = _db;
    }
    public new Result<User, string> CreateUser(string name,bool isOver18, string email, string password, Role role) => "Only admin can create users";

    public new Result<Account, string> CreateAccount(string name,AccountType accountType)
    {
      if (accountType == AccountType.Credit && !ctx.IsOver18) {
        return"You need to be over 18 to create a credit account";
      }
      return base.CreateAccount(name, accountType);

    }
    public override Account[] GetAccounts()
      => db.Accounts.Where(e => e.OwnerId == ctx.Id).ToArray();
    public override Transaction[] GetTransactions()
    {
      return db.Transactions.Where(e => e.SenderId == ctx.Id || e.ReciverId == ctx.Id).ToArray();
    }

    public override  Result<Account, string> CreateAccount(string name,AccountType accountType, User user)
    {
      return "Not Authorized";
    }
  }
}
