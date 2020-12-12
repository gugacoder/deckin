using System;
using System.Diagnostics;
using System.Linq;
using Keep.Paper.DesignX.Modeling;

namespace Keep.Paper.DesignX
{
  public class Page { }
  public class Sort { }

  public class User { }
  public class UserFilter { }
  public class UserEdit { }
  public class UserForm { }

  public interface IUserManager
  {
    User[] ListUsers(UserFilter filter, Page page, Sort sort);

    User CreateUser(UserForm form);

    User[] EditUser(UserEdit edit, User[] users);
  }


  public class _Sandbox
  {
    void Sample()
    {
      var view = new View();

      view.Resource = "MyFirstForm(Id=10)";
      view.Title = "My First Form";

      // DataSet é exibido como grade
      view.Disposition = new Disposition.Grid();

      // DataSet é exibido como formulário somente leitura
      // Rows no DataSet são tratados como Slides
      view.Disposition = new Disposition.Card();

      // DataSet é exibido como formulário editável
      // Rows no DataSet são tratados como Slides
      view.Disposition = new Disposition.Edit();

      view.Fields = new FieldCollection();

      var field1 = new Field();
      view.Fields.Add(field1);

      field1.Type = new FieldType.Variant();
      field1.Type = new FieldType.Text();

      view.Data = new Data.Object(new { Id = 10, Name = "Tenth" });
    }
  }
}
