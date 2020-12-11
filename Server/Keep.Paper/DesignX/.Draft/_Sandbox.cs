using System;
using System.Diagnostics;
using System.Linq;

namespace Keep.Paper.DesignX
{
  public class _Sandbox
  {
    void Sample()
    {
      var view = new View();

      view.Name = "MyFirstForm";
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
