using System;
using System.Data.Common;
using System.Data.SqlClient;
using Keep.Paper.Api;
using Keep.Paper.Data;
using Microsoft.Extensions.Configuration;

namespace AppSuite.Conectores
{
  public class DbDirector : DbConnector<DbDirector>
  {
    public DbDirector(IDbConnector dbConnector)
      : base("Director", dbConnector)
    {
    }
  }
}
