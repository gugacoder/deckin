using System;
using System.Collections.Generic;
using System.Text;

namespace Keep.Tools.Sequel
{
  [Flags]
  public enum Dialect
  {
    Undefined = 0,
    SqlServer = 1 << 1,
    SqlServerCe = 1 << 2,
    PostgreSql = 1 << 3,
    SQLite = 1 << 4,
    MySql = 1 << 5,
    Oracle = 1 << 6,
    HSqlDb = 1 << 7

    // Por vir
    // SqlServer2012 = SqlServer | 1 << 8,
    // SqlServer2014 = SqlServer | 1 << 9
  }
}
