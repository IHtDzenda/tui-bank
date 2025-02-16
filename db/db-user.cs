namespace Core.Db.Users
{
  public class UserOpetaion
  {
    private User ctx { get; set; }
    private DBContext db { get; set; }

    public UserOpetaion(User _ctx, DBContext _db)
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
    public Result<Account, string> CreateAccount(string name,AccountType accountType)
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


  }
  public class AdminUserOperations : UserOpetaion
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
  }
  public class ManagerUserOperations : UserOpetaion
  {

    private User ctx { get; set; }
    private DBContext db { get; set; }

    public ManagerUserOperations(User _ctx, DBContext _db) : base(_ctx, _db)
    {
      ctx = _ctx;
      db = _db;
    }
    public Result<User, string> CreateUser(string name,int age, string email, string password, Role role) => "Only admin can create users";

  }
  public class UserUserOperations : UserOpetaion
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
  }
}
