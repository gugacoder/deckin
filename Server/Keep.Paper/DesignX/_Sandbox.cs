using System;
using System.Diagnostics;
using System.Linq;

namespace Keep.Paper.DesignX
{
  public class _Sandbox
  {
    void Sample()
    {
      var form = new Form();

      form.Name = "MyFirstForm";
      form.Title = "My First Form";

      // DataSet é exibido como grade
      form.View = new View.Grid();

      // DataSet é exibido como formulário somente leitura
      // Rows no DataSet são tratados como Slides
      form.View = new View.Card();

      // DataSet é exibido como formulário editável
      // Rows no DataSet são tratados como Slides
      form.View = new View.Edit();

      form.Fields = new FieldCollection();

      var field1 = new Field();
      form.Fields.Add(field1);

      field1.Type = new FieldType.Variant();
      field1.Type = new FieldType.Text();

      var dataSet = new DataSet.Cached();
      dataSet.Add(new { Id = 10, Name = "Tenth" });
      form.Data = dataSet;

      foreach (var cell in form.Data.Rows.SelectMany(row => row.Cells))
      {

        form.Data.GetValue(0, 1);
        form.Data.GetValue(0, "one");

        form.Data.Rows[0].GetValue(1);
        form.Data.Rows[0].GetValue("one");

        form.Data.Rows[0].Cells[1].GetValue();
        form.Data.Rows[0].Cells["one"].GetValue();

        Debug.WriteLine(cell.ParentSet.Rows.Count);
        Debug.WriteLine(cell.ParentRow.Index);
        Debug.WriteLine(cell.Key);
        Debug.WriteLine(cell.Value);
      }
    }
  }
}
