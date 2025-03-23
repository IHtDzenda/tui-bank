using Core.Db;
using Core.Db.Users;
using Spectre.Console;
using static Core.Db.Accounts.AccountOperations;

namespace Core.Frontend
{
  public class FrontendEntryPoint
  {
    public State State { get; set; } = new State();
    private void Login()
    {
      AnsiConsole.MarkupLine("[blue]Welcome to the [red]Login[/] page![/]");
      var db = new DBContext();
      var users = db.Users.ToList();


      var id = AnsiConsole.Prompt(
          new SelectionPrompt<string>()
          .Title("Pick [green]account[/]?")
          .PageSize(10)
          .MoreChoicesText("[grey](And press enter to continue)[/]")
          .AddChoices(users.Select(e => $"{e.Id} > {e.Name} - {e.Email}").ToArray()));

      var SelectedUserId = id.Substring(0, 36);
      State.User = db.Users.Where(e => e.Id == Guid.Parse(SelectedUserId)).FirstOrDefault();
      State.Accounts = db.Accounts.Where(e => e.OwnerId == State.User.Id).ToArray();
      switch (State.User.Role)
      {
        case Role.Admin:
          State.UserOperations = new AdminUserOperations(State.User, db);
          break;
        case Role.Manager:
          State.UserOperations = new ManagerUserOperations(State.User, db);
          break;
        case Role.User:
          State.UserOperations = new UserUserOperations(State.User, db);
          break;
      }
      if (State.FirstOrSelectedAccount != null)
      {
        switch (State.FirstOrSelectedAccount.Type)
        {
          case AccountType.Checking:
            State.AccountOperations = new CheckingAccountOperations(State.User, State.FirstOrSelectedAccount, db);
            break;
          case AccountType.Savings:
            State.AccountOperations = new SavingsAccountOperations(State.User, State.FirstOrSelectedAccount, db);
            break;
          case AccountType.Credit:
            State.AccountOperations = new CreditAccountOperations(State.User, State.FirstOrSelectedAccount, db);
            break;
        }
      }
    }
    public virtual void App()
    {
      Login();
      switch (State.User.Role)
      {
        case Role.Admin:
          FrontendAdmin admin = new FrontendAdmin(State);
          admin.App();
          break;
        case Role.Manager:
          FrontendManager manager = new FrontendManager(State);
          manager.App();
          break;
        case Role.User:
          FrontendUser user = new FrontendUser(State);
          user.App();

          break;
      }
    }

  }
}
