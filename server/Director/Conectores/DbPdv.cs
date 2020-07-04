using System;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using Keep.Paper.Api;
using Keep.Tools.Sequel;
using Keep.Tools.Sequel.Runner;

namespace Director.Conectores
{
  public class DbPdv
  {
    private readonly DbDirector dbDirector;
    private readonly string templateDeStringDeConexao;

    public DbPdv(DbDirector dbDirector, string templateDeStringDeConexao)
    {
      this.dbDirector = dbDirector;
      this.templateDeStringDeConexao = templateDeStringDeConexao;
    }

    public string[] GetIps()
    {
      using var cn = dbDirector.GetConexao();
      cn.Open();
      var pdvs =
        @"select distinct DFendereco_rede
            from mlogic.vw_pdv
           where DFativo = 1"
          .AsSql()
          .Select<string>(cn);
      return pdvs;
    }

    public async Task<string[]> GetIpsAsync()
    {
      using var cn = dbDirector.GetConexao();
      await cn.OpenAsync();
      var pdvs = await
        @"select distinct DFendereco_rede
            from mlogic.vw_pdv
           where DFativo = 1"
          .AsSql()
          .SelectAsync<string>(cn);
      return pdvs;
    }

    public DbConnection GetConexao(string ip)
    {
      var stringDeConexao = this.templateDeStringDeConexao.Replace("{ip}", ip);
      return new Npgsql.NpgsqlConnection(stringDeConexao);
    }
  }
}
