using Core.Db;

namespace Core.Utils{

public static class Utils
{
  public static User auth(string? email,string? password)
  {
    if (email == null && password == null)
    {
      throw new ArgumentNullException("Email and Password are required.");
    }
    using var db = new DBContext();
    var  user = db.Users.Where(e => e.Email == email&& e.Password == password).FirstOrDefault();
    if (user == null)
    {
      throw new ArgumentException("Invalid credentials");
    }
    return user;
  }
}
}
