using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Keep.Tools.Sequel.Runner
{
  public class RecordReaderAsync : TransformReaderAsync<Record>
  {
    public new static readonly RecordReader Empty = new RecordReader(null);

    public static async Task<RecordReaderAsync> CreateAsync(
      Func<DbCommand> factory,
      CancellationToken stopToken)
    {
      var reader = new RecordReaderAsync();
      reader.Factory = factory;
      reader.Transform = reader => new Record(reader);
      await reader.ResetAsync(stopToken);
      return reader;
    }

    public int FieldCount
    {
      get { return (Reader != null) ? Reader.FieldCount : 0; }
    }

    public IEnumerable<int> Fields
    {
      get { return Enumerable.Range(0, FieldCount); }
    }

    public string GetFieldName(int fieldIndex)
    {
      if (Reader == null)
        return null;

      var name = Reader.GetName(fieldIndex);
      return name;
    }

    public int GetFieldIndex(string fieldName)
    {
      if (Reader == null)
        return -1;

      var index = Reader.GetOrdinal(fieldName);
      return index;
    }
  }
}
