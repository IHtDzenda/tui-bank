namespace Core.Db.Accounts
{
  public class AccountOperations
  {
    protected User ctx { get; }
    protected DBContext db { get; }
    protected Account account { get; }

    public AccountOperations(User _ctx, Account _account, DBContext _db)
    {
      ctx = _ctx;
      db = _db;
      account = _account;
    }

    public virtual Result<Transaction, string> SendMoney(Guid receiverId, int amount)
    {
      try
      {
        Account receivingAccount = db.Accounts.Find(receiverId);
        if (receivingAccount == null)
        {
          return "Receiver not found";
        }

        if (account.Balance < amount)
        {
          return "Insufficient funds";
        }

        Transaction transaction = new Transaction
        {
          Sender = account,
                 Reciver = receivingAccount,
                 Amount = amount,
                 Id = Guid.NewGuid(),
                 SenderId = account.Id,
                 ReciverId = receivingAccount.Id,
        };

        account.Balance -= amount;
        receivingAccount.Balance += amount;

        db.Transactions.Add(transaction);
        db.SaveChanges();
        return transaction;
      }
      catch (Exception e)
      {
        Console.WriteLine(e.Message);
        return "500";
      }
    }
  }

  public class CreditAccountOperations : AccountOperations
  {
    public CreditAccountOperations(User _ctx, Account _account, DBContext _db) 
      : base(_ctx, _account, _db) { }

    public override Result<Transaction, string> SendMoney(Guid receiverId, int amount)
    {
      return base.SendMoney(receiverId, amount);
    }
  }

  public class SavingsAccountOperations : AccountOperations
  {
    public SavingsAccountOperations(User _ctx, Account _account, DBContext _db) 
      : base(_ctx, _account, _db) { }

    public override Result<Transaction, string> SendMoney(Guid receiverId, int amount)
    {
      // Add savings-specific checks before calling base method
      if (account.Balance < amount) 
      {
        return "Not enough money";
      }
      return base.SendMoney(receiverId, amount);
    }
  }

  public class CheckingAccountOperations : AccountOperations
  {
    public CheckingAccountOperations(User _ctx, Account _account, DBContext _db) 
      : base(_ctx, _account, _db) { }

    public override Result<Transaction, string> SendMoney(Guid receiverId, int amount)
    {
      // Add savings-specific checks before calling base method
      if (account.Balance < amount) 
      {
        return "Not enough money";
      }
      return base.SendMoney(receiverId, amount);
    }
  }
}
