using System;
using Director.Models;
using Paper;
using Toolset;

namespace Director.Papers
{
  [Expose]
  public class LoginPaper : IPaper
  {
    private readonly LoginModel loginModel;

    public LoginPaper(LoginModel loginModel)
    {
      this.loginModel = loginModel;
    }

    public Ret Login(Domain.Login login)
    {
      return loginModel.Autenticar(login);
    }
  }
}
