using Spectre.Console;
using Spectre.Console.Rendering;

namespace Core.Frontend
{
  public class FrontendUser : FrontendEntryPoint
  {
    public enum Tabs
    {
      Overview = 0,
      Accounts = 1,
      Payments = 2,
    }

    public FrontendUser(State state)
    {
      State = state;
      CurrentTab = Tabs.Overview;
    }

    public Tabs CurrentTab { get; set; }

    public override void App()
    {
      while (true)
      {
        Console.Clear();
        switch (CurrentTab)
        {
          case Tabs.Overview:
            RenderUI(RenderUserInfo());
            break;
          case Tabs.Accounts:
            RenderUI(RenderAccounts()); 
            break;
          case Tabs.Payments:
            RenderUI(RenderPayments());
            break;
        }

        var key = Console.ReadKey();
        switch (key.Key)
        {
          case ConsoleKey.LeftArrow:
            if (CurrentTab - 1 >= 0)
              CurrentTab = CurrentTab - 1;
            break;
          case ConsoleKey.RightArrow:
            if (CurrentTab + 1 <= Tabs.Payments)
              CurrentTab = CurrentTab + 1;
            break;
          default:
            break;
        }
      }
    }

    void RenderUI(IRenderable data)
    {
      string[] tabs = ["Overview", "Accounts", "Pay"];
      var header = tabs.Select(e => e == tabs[(int)CurrentTab] ? $"[green]{e}[/]" : $"[grey]{e}[/]");
      var headerText = string.Join(" | ", header);

      var layout = new Layout("Root")
        .SplitColumns(
            new Layout("Left").SplitRows(
              new Layout("Top"),
              new Layout("Bottom"))
            );

      layout["Top"].Update(
          new Panel(
            Align.Left(data, VerticalAlignment.Top))
          .Expand().Header(headerText));

      layout["Bottom"].Update(
          new Panel(
            Align.Left(RenderUserInfo(), VerticalAlignment.Bottom))
          );

      // Render the layout
      AnsiConsole.Write(layout);
    }

    private IRenderable RenderUserInfo()
    {
      string[] text = [$"UserName: [green]{State.User.Name}[/]",
        $"Email: [green]{State.User.Email}[/]",
        $"Id: [green]{State.User.Id}[/]"];
      return new Markup(string.Join("\n", text));
    }

    private IRenderable RenderAccounts()
    {
      string[] accounts = new string[State.Accounts.Length];
      for (int i = 0; i < State.Accounts.Length; i++)
      {
        accounts[i] = $"[green]({i})[/] {State.Accounts[i].Name} \n   [grey]Balance: {State.Accounts[i].Balance}[/]";
      }

      return new Markup($"Accounts: \n{string.Join("\n", accounts)} \n\n");
    }

    private IRenderable RenderPayments()
    {
      var panel = new Panel(
          new Markup("[green]Make a Payment[/]\n\n[bold]Press 'Enter' to proceed[/]")
          ).Expand().Header("Payments", Justify.Center);

      AnsiConsole.Write(panel);

      if (Console.ReadKey().Key == ConsoleKey.Enter)
      {
        Console.Clear();

        var accountChoices = State.Accounts.Select((a,i) => $"{i}) {a.Name} (Balance: {a.Balance}€) #{a.Id}").ToList();
        int selectedAccountIndex = int.Parse(AnsiConsole.Prompt(
              new SelectionPrompt<string>()
              .Title("Select an account to send from:")
              .PageSize(10)
              .AddChoices(accountChoices))
            [0].ToString());
        State.SelectedAccountIndex = selectedAccountIndex;
        var selectedAccount = State.FirstOrSelectedAccount;

        var transactions = State.AccountOperations.GetTransactions();
        string[] pastRecipients = transactions.Select(e => e.Reciver.Name).ToArray().Concat(transactions.Select(e => e.Sender.Name)).ToArray();


        string recipient;
        do 
        {
          if (pastRecipients.Length > 0)
          {
            recipient = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                .Title("Select a past recipient or enter a new one:")
                .AddChoices(pastRecipients.Append("Enter new address"))
                );

            if (recipient == "Enter new address")
            {
              recipient = AnsiConsole.Ask<string>("Enter recipient address:");
            }
          }
          else
          {
            recipient = AnsiConsole.Ask<string>("Enter recipient address:");
          }

          if (!State.AccountOperations.ValaditeAccountNumber(recipient))
          {
            AnsiConsole.MarkupLine("[bold red]Invalid account number![/] [gray]press any key to tryagain[/]");
            Console.ReadKey();

          }
          Console.Clear();

        } while (!State.AccountOperations.ValaditeAccountNumber(recipient));

        decimal amount = AnsiConsole.Ask<int>("Enter amount:");
        bool confirm = AnsiConsole.Confirm($"Are you sure you want to send [green]{amount}[/] from [blue]{selectedAccount.Name}[/] to [yellow]{recipient}[/]?");

        if (confirm)
        {
          var res = State.AccountOperations.SendMoney(Guid.Parse(recipient), (int)amount);
          if (res.IsOk)
          {
            AnsiConsole.MarkupLine($"[bold green]Payment of {amount} sent from {selectedAccount.Name} to {recipient}![/]");
          }
          else
          {
            AnsiConsole.MarkupLine($"[bold red]Payment failed: {res.Error}[/]");
          }
          Thread.Sleep(5000);
        }
        else
        {
          AnsiConsole.MarkupLine("[bold red]Payment canceled.[/]");
        }

        AnsiConsole.MarkupLine("\nPress any key to return...");
        Console.ReadKey();
      }

      return new Markup("[bold]Payments Menu[/]\n\nNavigate using arrow keys.");
    }
  }
}
