
using Core.Db;
using Spectre.Console;

namespace Core.Frontend
{
  public class FrontendManager : FrontendEntryPoint
  {
    //2️⃣ Bankéři – Přístup ke všem účtům, přehled o celkovém objemu vkladů/úroků.
    public FrontendManager(State state)
    {
      State = state;
    }
    int index = 0;
    public enum  Tabs
    {
      Accounts = 0,
      Transactions = 1,
      BankStatus = 2,
    }
    public Tabs CurrentTab { get; set; }
      
    public  override void App()
    {
      while (true)
      {
        Console.Clear();
        switch (CurrentTab)
        {
          case Tabs.Accounts:
            RenderAccounts();
            break;
          case Tabs.Transactions:
            RenderTransactions();
            break;
          case Tabs.BankStatus:
            break;
        }
        var key = Console.ReadKey();
        switch (key.Key)
        {
          case ConsoleKey.LeftArrow:
            if (CurrentTab -1 >= 0)
              CurrentTab = CurrentTab - 1;
            break;
          case ConsoleKey.RightArrow:
            if ((int)CurrentTab + 1 <=  2)
              CurrentTab = CurrentTab + 1;
            break;

          default:
            break;
        }
      }
    }
    
    void RenderAccounts()
    {
      Account[] accounts = State.UserOperations.GetAccounts();
      string[] accountsText = new String[accounts.Length];
      var table = new Table();
      table.AddColumn("[BOLD]USER[/]");
      table.AddColumn("[BOLD]ACCOUNT NUMBER[/]");
      table.AddColumn("[BOLD]ACCOUNT TYPE[/]");
      table.AddColumn("[BOLD]BALANCE[/]");

      for (int i = 0; i < accounts.Length; i++)
      {
        string AccountType =  accounts[i].Type switch
        {
            Db.AccountType.Savings => "[bold yellow]Savings[/]",
            Db.AccountType.Checking =>"[bold yellow]Checking[/]",
            Db.AccountType.Credit => "[bold yellow]Credit[/]",
            _ => "[bold yellow]Unknown[/]",
        };
        table.AddRow(accounts[i].Owner.Name, accounts[i].Name, AccountType, accounts[i].Balance.ToString());
      }

      var layout = new Layout("Root");
      layout["Root"].Update(
          new Panel(
            Align.Center(table,VerticalAlignment.Bottom)).Header("[red]Accounts[/] ─ [gray]Transactions[/]")
          );
      AnsiConsole.Write(layout);
    }
    void RenderTransactions()
    {
      Transaction[] transactions = State.UserOperations.GetTransactions();
      string[] accountsText = new String[transactions.Length];
      var table = new Table();
      table.AddColumn("[bold]TRANSACTION ID[/]");
      table.AddColumn("[bold]SENDER[/]");
      table.AddColumn("[bold]RECEIVER[/]");
      table.AddColumn("[bold]AMOUNT[/]");

      for (int i = 0; i < transactions.Length; i++)
      {
        table.AddRow(transactions[i].Id.ToString(), 
            $"{transactions[i].Sender.Owner.Email} - |{transactions[i].Sender.Id}|" ,
            $"{transactions[i].Reciver.Owner.Email} - |{transactions[i].Reciver.Id}|" ,
            $"{transactions[i].Amount}€"
            );
      }

      var layout = new Layout("Root");
      layout["Root"].Update(
          new Panel(
            Align.Center(table,VerticalAlignment.Bottom)).Header("[gray]Accounts[/] ─ [red]Transactions[/]")
          );
      AnsiConsole.Write(layout);
    }
  }
}
