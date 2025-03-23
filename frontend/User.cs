using Spectre.Console;
using Spectre.Console.Rendering;

namespace Core.Frontend
{
  public class FrontendUser : FrontendEntryPoint
  {

    public enum  Tabs
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
    public  override void App()
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
            CurrentTab = CurrentTab + 1;
            break;

          default:
            break;
        }
      }

    }
    void RenderUI(IRenderable  data)
    {
      string[] tabs = ["Overview", "Accounts", "Settings", "Payments"];

      var layout = new Layout("Root")
        .SplitColumns(
            new Layout("Left").SplitRows(
              new Layout("Top"),
              new Layout("Bottom")),
            new Layout("Right")
            );
      

      layout["Top"].Update(
          new Panel(
            Align.Left(data,VerticalAlignment.Top))
          .Expand().Header("[green]User Info[/] [gray]User Info[/] [gray]Acoount[/] [gray]History[/] [gray]Logout[/]"));
      
      layout["Bottom"].Update(
          new Panel(
            Align.Left(RenderUserInfo(),VerticalAlignment.Bottom))
          );

      // Render the layout
      AnsiConsole.Write(layout);
    }
    private IRenderable RenderUserInfo() 
    {
      string[] text = [$"UserName: [green]{State.User.Name}[/]",
        $"Email: [green]{State.User.Email}[/]",
        $"Id: [green]{State.User.Id}[/]"];
      return new Markup(String.Join("\n", text));
    }
    private IRenderable RenderAccounts()
    {
      String[] accounts = new String[State.Accounts.Length];
      string helpMsg = "Select an account to view its balance via  arrow keys and press enter";
      for (int i = 0; i < State.Accounts.Length; i++)
      {
        accounts[i] = $"[green]({i})[/] {State.Accounts[i].Name} \n   [grey]Balance: {State.Accounts[i].Balance}[/]";
      }

      return new Markup($"Accounts: \n{String.Join("\n", accounts)} \n\n{helpMsg}");
    }

  }
}
