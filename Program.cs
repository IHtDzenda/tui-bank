using Core.Db;
using Core.Frontend;

public class Program
{
  static void Main(string[] args)
  {
    DbUtils.Init();
    FrontendEntryPoint Frontend = new FrontendEntryPoint();
    Frontend.App();
  }
}

