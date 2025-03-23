using Microsoft.EntityFrameworkCore;

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
        Account receivingAccount = db.Accounts.FirstOrDefault(a => a.Id == receiverId);
        if (receivingAccount == null)
        {
          return "Receiver not found";
        }


        Transaction transaction = new Transaction
        {
          Id = Guid.NewGuid(),  // Ensure unique ID
             Sender = account,
             Reciver = receivingAccount,
             Amount = amount,
             SenderId = account.Id,
             ReciverId = receivingAccount.Id
        };

        db.Entry(account).State = EntityState.Unchanged;
        db.Entry(receivingAccount).State = EntityState.Unchanged;

        account.Balance -= amount;
        receivingAccount.Balance += amount;

        db.Transactions.Add(transaction);
        db.SaveChanges();

        return transaction;
      }
      catch (Exception e)
      {
        Console.WriteLine($"Database Error: {e.InnerException?.Message ?? e.Message}");
        return e.Message;
      }
    }

    public bool ValaditeAccountNumber(string accountNumber)
    {
      try
      {
        Guid id = Guid.Parse(accountNumber);
        if (db.Accounts.FirstOrDefault(e => e.Id == id) == null)
        {
          return false;
        }
        return true;
      }
      catch 
      {
        return false;
      }

    }

    public Transaction[] GetTransactions()
    {
      return db.Transactions.Where(e => e.SenderId == ctx.Id || e.ReciverId == ctx.Id).ToArray();
    }

    public class CreditAccountOperations : AccountOperations
    {
      public CreditAccountOperations(User _ctx, Account _account, DBContext _db) 
        : base(_ctx, _account, _db) { }

      public override Result<Transaction, string> SendMoney(Guid receiverId, int amount)
      {

        if (account.Balance + account.WeeklyLimit < amount) 
        {
          return "Not enough money";
        }
        return base.SendMoney(receiverId, amount);
      }
    }

    public class SavingsAccountOperations : AccountOperations
    {
      public SavingsAccountOperations(User _ctx, Account _account, DBContext _db) 
        : base(_ctx, _account, _db) { }

      public override Result<Transaction, string> SendMoney(Guid receiverId, int amount)
      {
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
        if (account.Balance < amount) 
        {
          return "Not enough money";
        }
        return base.SendMoney(receiverId, amount);
      }
    }
  }
}
