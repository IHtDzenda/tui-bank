using Core.Db;
using Core.Db.Accounts;
using Core.Db.Users;

namespace Core.Frontend
{
  public class State
  {
    public Account[] Accounts = new Account[] { };

    public DBContext db = new DBContext();
    public AccountOperations AccountOperations { get; set; }
    public UserOpetaions UserOperations { get; set; }
    public User User { get; set; }

    private int _selectedAccountIndex = -1;

    public int SelectedAccountIndex
    {
      get => _selectedAccountIndex; 
      set
      {
        if (_selectedAccountIndex == value) return;

        _selectedAccountIndex = value;

        if (_selectedAccountIndex >= 0 && _selectedAccountIndex < Accounts.Length)
        {
          switch (Accounts[_selectedAccountIndex].Type)
          {
            case AccountType.Checking:
              AccountOperations = new CheckingAccountOperations(User, Accounts[_selectedAccountIndex], db);
              break;
            case AccountType.Savings:
              AccountOperations = new SavingsAccountOperations(User, Accounts[_selectedAccountIndex], db);
              break;
            case AccountType.Credit:
              AccountOperations = new CreditAccountOperations(User, Accounts[_selectedAccountIndex], db);
              break;
          }
        }
      }
    }

    public Account FirstOrSelectedAccount
    {
      get => _selectedAccountIndex >= 0 && _selectedAccountIndex < Accounts.Length
        ? Accounts[_selectedAccountIndex]
        : Accounts.Length > 0 ? Accounts[0] : null;
    }
  }

}
