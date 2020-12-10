using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Keep.Tools;
using Keep.Tools.Collections;
using Keep.Tools.Reflection;

namespace Keep.Paper.DesignX
{
  public abstract class DataSet
  {
    public abstract Set<Row> Rows { get; }

    public object GetValue(int row, int index) => Rows[row].GetValue(index);

    public object GetValue(int row, string key) => Rows[row].GetValue(key);

    public T GetValue<T>(int row, int index) => Rows[row].GetValue<T>(index);

    public T GetValue<T>(int row, string key) => Rows[row].GetValue<T>(key);

    public class Cached : DataSet
    {
      public override Set<Row> Rows { get; } = new Set<Row>();

      public Cached(IEnumerable items = null)
      {
        items?.Cast<object>().ForEach(Add);
      }

      public void Add(object @object)
      {
        Row row = null;
        row = @object is IDictionary map
          ? (Row)new Row.Map(this, map, () => Rows.IndexOf(row))
          : (Row)new Row.Object(this, @object, () => Rows.IndexOf(row));
        Rows.Add(row);
      }
    }

    public class Table : DataSet
    {
      public override Set<Row> Rows { get; }

      public Table(DataTable table)
      {
        var list = table.Rows
          .Cast<DataRow>()
          .Select((_, index) =>
          {
            Row row = null;
            row = new Row.Record(this, table, index, () => Rows.IndexOf(row));
            return row;
          })
          .ToList();
        this.Rows = new Set<Row>(list);
      }
    }
  }
}
