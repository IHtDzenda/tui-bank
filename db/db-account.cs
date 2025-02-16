namespace Core.Db.Accounts
{
  public class AccountOperations
  {

    private User ctx { get; set; }
    private DBContext db { get; set; }
    private Account acount { get; set; }

    public AccountOperations(User _ctx, DBContext _db, Account _acount)
    {
      ctx = _ctx;
      db = _db;
      acount = _acount;
      
    }
    public Result<Transaction, string> SendMoney(Guid reciverId, int amount)
    {
      try
      {
        Account reciveringAccount = db.Accounts.Find(reciverId);
        if (reciveringAccount== null)
        {
          return "Reciver not found";
        }

        Transaction transaction = new Transaction
        {
          Sender = acount ,
          Reciver = reciveringAccount,
          Amount = amount,
          Id = Guid.NewGuid(),
          SenderId = acount.Id,
          ReciverId = reciveringAccount.Id,
        };
    
        db.Transactions.Add(transaction);
        var updatedSender  =db.Accounts.Find(acount.Id);
        var updatedReciver = db.Accounts.Find(reciverId);
        if (updatedSender == null || updatedReciver == null)
        {
          return "Account not found";
        }
        updatedSender.Balance -= amount;
        updatedReciver.Balance += amount;
        db.Update(updatedSender);
        db.Update(updatedReciver);
        db.SaveChanges();
        return transaction;
      }
      catch (System.Exception e)
      {
          Console.WriteLine(e.Message);
          return "500";
      }
    }
  }
  public class CreditAccountOperations : AccountOperations
  {
    private User ctx { get; set; }
    private DBContext db { get; set; }
    private Account acount { get; set; }

    public CreditAccountOperations(User _ctx, DBContext _db, Account _acount) : base(_ctx, _db, _acount)
    {
      ctx = _ctx;
      db = _db;
      acount = _acount;
      
    }
    public new Result<Transaction, string> SendMoney(Guid reciverId, int amount) => base.SendMoney(reciverId, amount);
  }
public class SavingsAccountOperations : AccountOperations
  {
    private User ctx { get; set; }
    private DBContext db { get; set; }
    private Account acount { get; set; }

    public SavingsAccountOperations(User _ctx, DBContext _db, Account _acount) : base(_ctx, _db, _acount)
    {
      ctx = _ctx;
      db = _db;
      acount = _acount;
      
    }
    public new Result<Transaction, string> SendMoney(Guid reciverId, int amount)
    { 
      if (acount.Balance < amount) {
        return "Not enough money";
      }
      return base.SendMoney(reciverId, amount);
    }
  }
}

