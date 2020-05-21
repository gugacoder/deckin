//using System;
//using System.Threading.Tasks;

//namespace Keep.Paper.Papers
//{
//  public class LoginPaper
//  {
//    public object Data() => new
//    {
//      Username = "",
//      Password = ""
//    };

//    public object View() => new
//    {
//      Template = "Form",
//      Fields = new
//      {
//        Username = new
//        {
//          Title = "Usuário"
//        },
//        Password = new
//        {
//          Title = "Senha",
//          Password = true
//        }
//      }
//    };

//    public object Actions() => new
//    {
//      Authenticate = new
//      {
//        Title = "Autenticar",
//        Paper = nameof(AuthenticateAsync),
//        Primary = true
//      }
//    };

//    public object Links() => new
//    {
//      PasswordRecovery = new
//      {
//        Title = "Esqueceu sua senha?",
//        Paper = nameof(PasswordRecoveryPaper)
//      }
//    };

//    public object Authenticate(object data) => throw new NotImplementedException();
//  }

//  class PasswordRecoveryPaper { }
//}
