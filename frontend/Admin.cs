using Core.Db;
using Spectre.Console;

namespace Core.Frontend
{
  public class FrontendAdmin : FrontendEntryPoint
  {
    public FrontendAdmin(State state)
    {
      State = state;
    }
    public int UserIndex { get; set; }
    public User[] Users { get; set; }
    public override void App()
    {
      Users = State.UserOperations.GetUsers();
      State.Accounts = State.UserOperations.GetAccounts();
      while (true)
      {
        Console.Clear();
        RenderUsers();
        RenderHelp();
        var key = Console.ReadKey();
        switch (key.Key)
        {
          case ConsoleKey.UpArrow:
            if (UserIndex > 0) UserIndex--;
            break;
          case ConsoleKey.DownArrow:
            if (Users.Length > UserIndex + 1) UserIndex++;
            break;
          case ConsoleKey.A:
            AddUser();
            break;
          case ConsoleKey.D:
            DeleteUser();
            break;
          default:
            break;
        }
      }
    }

    void RenderUsers()
    {
      Table userTable = new Table();
      userTable.AddColumn("[BOLD]USER[/]");
      userTable.AddColumn("[BOLD]EMAIL[/]");
      userTable.AddColumn("[BOLD]ROLE[/]");

      for (int i = 0; i < Users.Length; i++)
      {
        string styles = UserIndex == i ? "[red bold underline]" : "[]";
        string selectedArrow = UserIndex == i ? "->" : "  ";
        userTable.AddRow($"{styles}{selectedArrow} {Users[i].Name}[/]", 
            $"{styles}{Users[i].Email}[/]", 
            $"{styles}{Users[i].Role }[/]");
      }

      var panel = new Panel(Align.Center(userTable));
      panel.Border = BoxBorder.Rounded;
      panel.Header = new PanelHeader("User List", Justify.Center);
      
      AnsiConsole.Write(panel);

      RenderUserAccounts();
    }

    void RenderUserAccounts()
    {
      var selectedUser = Users[UserIndex].Id;
      Account[] accounts = State.Accounts.Where(e => e.OwnerId == selectedUser).ToArray();
      Table accountTable = new Table();
      accountTable.AddColumn("[BOLD]ACCOUNT NAME[/]");
      accountTable.AddColumn("[BOLD]ACCOUNT TYPE[/]");
      accountTable.AddColumn("[BOLD]BALANCE[/]");

      foreach (var account in accounts)
      {
        string accountType = account.Type switch
        {
          Db.AccountType.Savings => "[bold]Savings[/]",
          Db.AccountType.Checking => "[bold]Checking[/]",
          Db.AccountType.Credit => "[bold]Credit[/]",
          _ => "[bold yellow]Unknown[/]",
        };
        accountTable.AddRow(account.Name, accountType, account.Balance.ToString());
      }
      
      var panel = new Panel(Align.Center(accountTable));
      panel.Border = BoxBorder.Rounded;
      panel.Header = new PanelHeader("Account Details", Justify.Center);
      AnsiConsole.Write(panel);
    }
    public void RenderHelp()
    {
      var panel = new Panel(Align.Center(new Markup("[green]A[/] - Add a new user\n[green]D[/] - Delete a user")));
      panel.Border = BoxBorder.Rounded;
      panel.Header = new PanelHeader("Help", Justify.Center);
      AnsiConsole.Write(panel);
    }
    public void AddUser()
    {
      Console.Clear();
      AnsiConsole.MarkupLine("[blue]Adding a new user[/]");
      string name = AnsiConsole.Ask<string>("What's the user's [green]name [/]?");
      string email = AnsiConsole.Ask<string>("What's the user's email?");
      string password = AnsiConsole.Ask<string>("What's the user's password?");
      bool isOver18 = AnsiConsole.Prompt(
          new TextPrompt<string>("Is the user over 18?")
          .AddChoices(["Yes", "No"])) == "Yes";
      Role role = AnsiConsole.Prompt(
          new TextPrompt<string>("What is the user's role?")
          .AddChoices(["Admin", "Manager", "User"])) switch
      {
        "Admin" => Db.Role.Admin,
        "Manager" => Db.Role.Manager,
        _ => Db.Role.User,
      };
      var res = State.UserOperations.CreateUser(name, isOver18, email, password, role);
      if (res.IsOk)
      {
        Users = State.UserOperations.GetUsers();
      }
    }
    public void DeleteUser()
    {
      Console.Clear();
      AnsiConsole.MarkupLine($"[blue]Deleting a user {Users[UserIndex].Name } - { Users[UserIndex].Email}[/]");
      bool areYouSure = AnsiConsole.Prompt(
          new TextPrompt<string>("Are you sure?")
          .AddChoices(["Yes", "No"])) == "Yes";
      if (areYouSure) 
      {
        State.UserOperations.DeleteUser(Users[UserIndex].Id);
      }
      Users = State.UserOperations.GetUsers();
      UserIndex = 0;

    }
  }
}
