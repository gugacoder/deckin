using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using Keep.Paper.Api;
using Keep.Paper.Data;
using Keep.Paper.Services;
using Keep.Tools;
using Keep.Tools.Collections;
using Keep.Tools.Data;
using Keep.Tools.Sequel;
using Keep.Tools.Sequel.Runner;
using Microsoft.Extensions.Configuration;

namespace Director.Conectores
{
  public class DbPdv : DbConnector<DbPdv>
  {
    public DbPdv(IDbConnector dbConnector)
      : base("Pdv", dbConnector)
    {
      this.SetProvider(DataProviders.PostgreSQL);
      this.SetConnectionString("Server=;Database=DBPDV;User ID=postgres;Password=local;Timeout=60");
    }
  }
}
