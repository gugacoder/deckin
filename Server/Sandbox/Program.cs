using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using Keep.Tools;
using Keep.Tools.Sequel;
using Keep.Tools.Sequel.Runner;

#nullable enable

namespace Director
{
  public class Program
  {
    public static void Main(string[] args)
    {
      try
      {
        using var cn = new SqlConnection(
          "server=172.27.0.119;database=DBdirector_mac_29;uid=sl;pwd=123;timeout=60;"
        );
        cn.Open();

        SequelTracer.TraceQuery = sql => Debug.WriteLine(sql);

        var codigos = @"
          select TBempresa_mercadologic.DFcod_empresa
            from TBempresa_mercadologic
           inner join TBempresa
                   on TBempresa.DFcod_empresa = TBempresa_mercadologic.DFcod_empresa
           where TBempresa_mercadologic.DFcod_empresa matches if set @DFcod_empresa
             and TBempresa.DFnome_fantasia matches if set @DFnome_fantasia"
            .AsSql()
            .Set(new
            {
              //DFcod_empresa = (int?)null,
              DFnome_fantasia = (string?)null
            })
            .Select<int>(cn);

        Debug.WriteLine(string.Join(", ", codigos));
      }
      catch (Exception ex)
      {
        ex.Trace();
      }
    }
  }
}
