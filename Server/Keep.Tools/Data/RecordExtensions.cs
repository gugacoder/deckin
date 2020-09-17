using System;
using System.Data;

namespace Keep.Tools.Data
{
  public static class RecordExtensions
  {
    public static string[] GetFieldNames(this IDataRecord record)
    {
      var names = new string[record.FieldCount];
      for (var i = 0; i < record.FieldCount; i++)
      {
        names[i] = record.GetName(i);
      }
      return names;
    }

    public static bool GetBoolean(this IDataRecord record, string name) { return record.GetBoolean(record.GetOrdinal(name)); }
    public static byte GetByte(this IDataRecord record, string name) { return record.GetByte(record.GetOrdinal(name)); }
    public static long GetBytes(this IDataRecord record, string name, long fieldOffset, byte[] buffer, int bufferoffset, int length) { return record.GetBytes(record.GetOrdinal(name), fieldOffset, buffer, bufferoffset, length); }
    public static char GetChar(this IDataRecord record, string name) { return record.GetChar(record.GetOrdinal(name)); }
    public static long GetChars(this IDataRecord record, string name, long fieldOffset, char[] buffer, int bufferoffset, int length) { return record.GetChars(record.GetOrdinal(name), fieldOffset, buffer, bufferoffset, length); }
    public static IDataReader GetData(this IDataRecord record, string name) { return record.GetData(record.GetOrdinal(name)); }
    public static string GetDataTypeName(this IDataRecord record, string name) { return record.GetDataTypeName(record.GetOrdinal(name)); }
    public static DateTime GetDateTime(this IDataRecord record, string name) { return record.GetDateTime(record.GetOrdinal(name)); }
    public static decimal GetDecimal(this IDataRecord record, string name) { return record.GetDecimal(record.GetOrdinal(name)); }
    public static double GetDouble(this IDataRecord record, string name) { return record.GetDouble(record.GetOrdinal(name)); }
    public static Type GetFieldType(this IDataRecord record, string name) { return record.GetFieldType(record.GetOrdinal(name)); }
    public static float GetFloat(this IDataRecord record, string name) { return record.GetFloat(record.GetOrdinal(name)); }
    public static Guid GetGuid(this IDataRecord record, string name) { return record.GetGuid(record.GetOrdinal(name)); }
    public static short GetInt16(this IDataRecord record, string name) { return record.GetInt16(record.GetOrdinal(name)); }
    public static int GetInt32(this IDataRecord record, string name) { return record.GetInt32(record.GetOrdinal(name)); }
    public static long GetInt64(this IDataRecord record, string name) { return record.GetInt64(record.GetOrdinal(name)); }
    public static string GetString(this IDataRecord record, string name) { return record.GetString(record.GetOrdinal(name)); }
    public static object GetValue(this IDataRecord record, string name) { return record.GetValue(record.GetOrdinal(name)); }
    public static bool IsDBNull(this IDataRecord record, string name) { return record.IsDBNull(record.GetOrdinal(name)); }
  }
}
