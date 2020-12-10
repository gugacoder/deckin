using System;
using System.Diagnostics;
using Keep.Tools;
using Keep.Tools.Collections;
using System.Linq;
using Keep.Paper.Draft;

namespace Mercadologic.Replicacao
{
  public class Program
  {
    public static void Main(string[] args)
    {
      try
      {

        var data = new DataSet.Cached();
        data.Add(new { Id = 10, Name = "Tenth" });
        data.Add(new { Id = 20, Name = "Twenty" });
        data.Add(new { Id = 30, Name = "Thirty", Age = 3000 });

        Debug.WriteLine(string.Join(",", data.Rows.FirstOrDefault().Keys));
        Debug.WriteLine(string.Join(",", data.Rows.LastOrDefault().Keys));

        Debug.WriteLine(data.GetValue(0, 0));
        Debug.WriteLine(data.GetValue(0, "Name"));
        
        Debug.WriteLine(data.Rows[0].GetValue(0));
        Debug.WriteLine(data.Rows[0].GetValue("Name"));
        
        Debug.WriteLine(data.Rows[0].Cells[0].GetValue());
        Debug.WriteLine(data.Rows[0].Cells["Name"].GetValue());

        foreach (var cell in data.Rows.SelectMany(row => row.Cells))
        {
          Debug.WriteLine("- - -");
          Debug.WriteLine($"RowCount: {cell.ParentSet.Rows.Count}");
          Debug.WriteLine($"RowIndex: {cell.ParentRow.Index}");
          Debug.WriteLine($"ColCount: {cell.ParentRow.Cells.Count}");
          Debug.WriteLine($"ColIndex: {cell.Index}");
          Debug.WriteLine($"ColValue: {cell.GetValue()}");
        }

      }
      catch (Exception ex)
      {
        Console.WriteLine("FAIL!!!");
        Console.WriteLine(ex.GetCauseMessage());
        Console.WriteLine(ex.GetStackTrace());
      }
    }
  }
}
