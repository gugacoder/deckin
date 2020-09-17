using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Keep.Tools.Sequel.Runner
{
  public class Record : IDataRecord
  {
    private readonly IDataRecord record;

    public Record(IDataRecord record)
    {
      this.record = record;
    }

    public int FieldCount { get { return record.FieldCount; } }

    public object this[int i] { get { return record[i]; } }
    public object this[string name] { get { return record[name]; } }

    public string[] GetFieldNames()
    {
      var names = new string[record.FieldCount];
      for (var i = 0; i < record.FieldCount; i++)
      {
        names[i] = record.GetName(i);
      }
      return names;
    }

    public bool GetBoolean(int i) { return record.GetBoolean(i); }
    public byte GetByte(int i) { return record.GetByte(i); }
    public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length) { return record.GetBytes(i, fieldOffset, buffer, bufferoffset, length); }
    public char GetChar(int i) { return record.GetChar(i); }
    public long GetChars(int i, long fieldOffset, char[] buffer, int bufferoffset, int length) { return record.GetChars(i, fieldOffset, buffer, bufferoffset, length); }
    public IDataReader GetData(int i) { return record.GetData(i); }
    public string GetDataTypeName(int i) { return record.GetDataTypeName(i); }
    public DateTime GetDateTime(int i) { return record.GetDateTime(i); }
    public decimal GetDecimal(int i) { return record.GetDecimal(i); }
    public double GetDouble(int i) { return record.GetDouble(i); }
    public Type GetFieldType(int i) { return record.GetFieldType(i); }
    public float GetFloat(int i) { return record.GetFloat(i); }
    public Guid GetGuid(int i) { return record.GetGuid(i); }
    public short GetInt16(int i) { return record.GetInt16(i); }
    public int GetInt32(int i) { return record.GetInt32(i); }
    public long GetInt64(int i) { return record.GetInt64(i); }
    public string GetName(int i) { return record.GetName(i); }
    public int GetOrdinal(string name) { return record.GetOrdinal(name); }
    public string GetString(int i) { return record.GetString(i); }
    public object GetValue(int i) { return record.GetValue(i); }
    public int GetValues(object[] values) { return record.GetValues(values); }
    public bool IsDBNull(int i) { return record.IsDBNull(i); }

    public bool GetBoolean(string name) { return record.GetBoolean(record.GetOrdinal(name)); }
    public byte GetByte(string name) { return record.GetByte(record.GetOrdinal(name)); }
    public long GetBytes(string name, long fieldOffset, byte[] buffer, int bufferoffset, int length) { return record.GetBytes(record.GetOrdinal(name), fieldOffset, buffer, bufferoffset, length); }
    public char GetChar(string name) { return record.GetChar(record.GetOrdinal(name)); }
    public long GetChars(string name, long fieldOffset, char[] buffer, int bufferoffset, int length) { return record.GetChars(record.GetOrdinal(name), fieldOffset, buffer, bufferoffset, length); }
    public IDataReader GetData(string name) { return record.GetData(record.GetOrdinal(name)); }
    public string GetDataTypeName(string name) { return record.GetDataTypeName(record.GetOrdinal(name)); }
    public DateTime GetDateTime(string name) { return record.GetDateTime(record.GetOrdinal(name)); }
    public decimal GetDecimal(string name) { return record.GetDecimal(record.GetOrdinal(name)); }
    public double GetDouble(string name) { return record.GetDouble(record.GetOrdinal(name)); }
    public Type GetFieldType(string name) { return record.GetFieldType(record.GetOrdinal(name)); }
    public float GetFloat(string name) { return record.GetFloat(record.GetOrdinal(name)); }
    public Guid GetGuid(string name) { return record.GetGuid(record.GetOrdinal(name)); }
    public short GetInt16(string name) { return record.GetInt16(record.GetOrdinal(name)); }
    public int GetInt32(string name) { return record.GetInt32(record.GetOrdinal(name)); }
    public long GetInt64(string name) { return record.GetInt64(record.GetOrdinal(name)); }
    public string GetString(string name) { return record.GetString(record.GetOrdinal(name)); }
    public object GetValue(string name) { return record.GetValue(record.GetOrdinal(name)); }
    public bool IsDBNull(string name) { return record.IsDBNull(record.GetOrdinal(name)); }
  }
}