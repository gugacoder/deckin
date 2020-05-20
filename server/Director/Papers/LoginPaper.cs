using System;
using Director.Connectors;
using Director.Models;
using Keep.Paper;
using Keep.Tools;

namespace Director.Papers
{
  [Expose]
  public class LoginPaper : IPaper
  {
    private readonly DirectorDbContext dbDirector;

    public LoginPaper(DirectorDbContext dbDirector)
    {
      this.dbDirector = dbDirector;
    }

    public Ret Login(Domain.Login login)
    {
      return new LoginModel(dbDirector).Autenticar(login);
    }
  }
}
