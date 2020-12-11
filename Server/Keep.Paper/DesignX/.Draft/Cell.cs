using System;
using System.Collections.Generic;
using Keep.Tools;

namespace Keep.Paper.DesignX
{
  public class Cell
  {
    private readonly Func<int> indexer;

    internal Cell(Row parentRow, string key, Func<int> indexer)
    {
      this.ParentRow = parentRow;
      this.Key = key;
      this.indexer = indexer;
    }

    public DataSet ParentSet => ParentRow.ParentSet;

    public Row ParentRow { get; }

    public string Key { get; }

    public int Index => indexer.Invoke();

    public object Value => ParentRow.GetValue(Key);

    public object GetValue() => ParentRow.GetValue(Key);

    public object GetValue<T>() => ParentRow.GetValue<T>(Key);
  }
}
