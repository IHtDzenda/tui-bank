using Core.Db;
using Spectre.Console;

namespace Core.Frontend
{
  public class FrontendManager : FrontendEntryPoint
  {
    public FrontendManager(State state)
    {
      State = state;
    }

    int index = 0;
    public enum Tabs
    {
      Accounts = 0,
      Transactions = 1,
      BankStatus = 2,
      DebtUsers = 3
    }
    
    public Tabs CurrentTab { get; set; }

    public override void App()
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
            RenderBankStatus();
            break;
          case Tabs.DebtUsers:
            RenderDebtUsers();
            break;
        }

        var key = Console.ReadKey();
        switch (key.Key)
        {
          case ConsoleKey.LeftArrow:
            if (CurrentTab - 1 >= 0)
              CurrentTab--;
            break;
          case ConsoleKey.RightArrow:
            if ((int)CurrentTab + 1 <= 3)
              CurrentTab++;
            break;
          case ConsoleKey.S:
            SimulateMonth();
            break;
          case ConsoleKey.C:
            if (CurrentTab != Tabs.BankStatus)
              break;
            State.UserOperations.ClockIntrest();
            break;
          default:
            break;
        }
      }
    }

    void RenderAccounts()
    {
      Account[] accounts = State.UserOperations.GetAccounts();
      var table = new Table();
      table.AddColumn("[BOLD]USER[/]");
      table.AddColumn("[BOLD]ACCOUNT NUMBER[/]");
      table.AddColumn("[BOLD]ACCOUNT TYPE[/]");
      table.AddColumn("[BOLD]BALANCE[/]");

      foreach (var account in accounts)
      {
        string accountType = account.Type switch
        {
            Db.AccountType.Savings => "[bold yellow]Savings[/]",
            Db.AccountType.Checking => "[bold yellow]Checking[/]",
            Db.AccountType.Credit => "[bold yellow]Credit[/]",
            _ => "[bold yellow]Unknown[/]",
        };
        table.AddRow(account.Owner.Name, account.Name, accountType, $"{account.Balance}€");
      }

      var layout = new Layout("Root");
      layout["Root"].Update(
          new Panel(Align.Center(table, VerticalAlignment.Bottom))
          .Header("[red]Accounts[/] ─ [gray]Transactions[/] ─ [gray]Bank Status[/] ─ [gray]Debt Users[/]")
      );
      AnsiConsole.Write(layout);
    }

    void RenderTransactions()
    {
      Transaction[] transactions = State.UserOperations.GetTransactions();
      var table = new Table();
      table.AddColumn("[bold]TRANSACTION ID[/]");
      table.AddColumn("[bold]SENDER[/]");
      table.AddColumn("[bold]RECEIVER[/]");
      table.AddColumn("[bold]AMOUNT[/]");

      foreach (var transaction in transactions)
      {
        table.AddRow(transaction.Id.ToString(), 
            $"{transaction.Sender.Owner.Email} - |{transaction.Sender.Id}|",
            $"{transaction.Reciver.Owner.Email} - |{transaction.Reciver.Id}|",
            $"{transaction.Amount}€");
      }

      var layout = new Layout("Root");
      layout["Root"].Update(
          new Panel(Align.Center(table, VerticalAlignment.Bottom))
          .Header("[gray]Accounts[/] ─ [red]Transactions[/] ─ [gray]Bank Status[/] ─ [gray]Debt Users[/]")
      );
      AnsiConsole.Write(layout);
    }

    void RenderBankStatus()
    {
      Account[] accounts = State.UserOperations.GetAccounts();
      decimal totalDeposits = accounts.Sum(a => a.Balance);
      decimal totalInterest = accounts.Where(a => a.Balance > 0)
                                      .Sum(a => a.Balance * 0.02m);

      var table = new Table();
      table.AddColumn("[bold]Total Deposits[/]");
      table.AddColumn("[bold]Estimated Interest[/]");
      table.AddRow($"{totalDeposits}€", $"{totalInterest}€");

      var layout = new Layout("Root");
      layout["Root"].Update(
          new Panel(Align.Center(table, VerticalAlignment.Bottom))
          .Header("[gray]Accounts[/] ─ [gray]Transactions[/] ─ [red]Bank Status[/] ─ [gray]Debt Users[/]")
      );
      AnsiConsole.Write(layout);
    }

    void RenderDebtUsers()
    {
      Account[] accounts = State.UserOperations.GetAccounts();
      var debtAccounts = accounts.Where(a => a.Balance < 0).ToArray();

      var table = new Table().Caption("To simulate a month, press 'S'");
      table.AddColumn("[BOLD]USER[/]");
      table.AddColumn("[BOLD]ACCOUNT NUMBER[/]");
      table.AddColumn("[BOLD]BALANCE[/]");

      foreach (var account in debtAccounts)
      {
        table.AddRow(account.Owner.Name, account.Name, $"[red]{account.Balance}€[/]");
      }

      var layout = new Layout("Root");
      layout["Root"].Update(
          new Panel(Align.Center(table, VerticalAlignment.Bottom))
          .Header("[gray]Accounts[/] ─ [gray]Transactions[/] ─ [gray]Bank Status[/] ─ [red]Debt Users[/]")
      );
      AnsiConsole.Write(layout);
    }

    void SimulateMonth()
    {
      State.UserOperations.ClockIntrest();
    }
  }
}
