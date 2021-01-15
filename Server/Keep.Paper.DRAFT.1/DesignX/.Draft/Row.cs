using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Keep.Tools;
using Keep.Tools.Reflection;

namespace Keep.Paper.DesignX
{
  public abstract class Row
  {
    private readonly Func<int> indexer;
    private Set.Keyed<Cell> _cells;

    protected Row(DataSet parentSet, Func<int> indexer)
    {
      this.ParentSet = parentSet;
      this.indexer = indexer;
    }

    public int Index => indexer.Invoke();

    public DataSet ParentSet { get; }

    public abstract ICollection<string> Keys { get; }

    public Set.Keyed<Cell> Cells
    {
      get
      {
        if (_cells == null)
        {
          var items = Keys.Select(key =>
          {
            Cell cell = null;
            cell = new Cell(this, key, () => this.Cells.IndexOf(cell));
            return cell;
          });

          _cells = new Set.Keyed<Cell>(items, (set, key) =>
            set.FirstOrDefault((cell) => cell.Key == key));
        }
        return _cells;
      }
    }

    public object GetValue(int index)
    {
      var key = Keys.ElementAtOrDefault(index);
      return key != null ? GetValue(key) : null;
    }

    public abstract object GetValue(string key);

    public T GetValue<T>(int index) => Change.To<T>(GetValue(index));

    public T GetValue<T>(string key) => Change.To<T>(GetValue(key));

    public class Map : Row
    {
      private readonly IDictionary map;
      private string[] _keys;

      public Map(DataSet dataSet, IDictionary map, Func<int> indexer)
        : base(dataSet, indexer)
      {
        this.map = map;
      }

      public override ICollection<string> Keys
        => _keys ??= map.Keys.Cast<string>().ToArray();

      public override object GetValue(string key)
        => map.Contains(key) ? map[key] : null;
    }

    public class Object : Row
    {
      private readonly object @object;
      private string[] _keys;

      public Object(DataSet dataSet, object @object, Func<int> indexer)
        : base(dataSet, indexer)
      {
        this.@object = @object;
      }

      public override ICollection<string> Keys
        => _keys ??= @object._Keys().ToArray();

      public override object GetValue(string key)
        => @object._Get(key);
    }

    public class Record : Row
    {
      private readonly DataTable table;
      private readonly int row;
      private string[] _keys;

      public Record(DataSet dataSet, DataTable table, int row, Func<int> indexer)
        : base(dataSet, indexer)
      {
        this.table = table;
        this.row = row;
      }

      public override ICollection<string> Keys
        => _keys ??= table.Columns
            .Cast<DataColumn>()
            .Select(column => column.ColumnName)
            .ToArray();

      public override object GetValue(string key)
        => table.Rows[row][key];
    }
  }

}
