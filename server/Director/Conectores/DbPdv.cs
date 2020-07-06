using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using Keep.Paper.Api;
using Keep.Tools;
using Keep.Tools.Collections;
using Keep.Tools.Sequel;
using Keep.Tools.Sequel.Runner;

namespace Director.Conectores
{
  public class DbPdv
  {
    private readonly string stringDeConexao;
    private readonly string template;

    public DbPdv(string stringDeConexao)
    {
      this.stringDeConexao = stringDeConexao;
      this.template = CriarTemplate(stringDeConexao);
    }

    private string CriarTemplate(string stringDeConexao)
    {
      var entradas =
        from parametro in stringDeConexao.Split(";")
        let partes = parametro.Split('=')
        let chave = partes.First()
        let valorBase = string.Join("=", partes.Skip(1))
        let valor = chave.EqualsAnyIgnoreCase("server", "data source", "host")
          ? "{servidor}" : valorBase
        select $"{chave}={valor}";

      var template = string.Join(";", entradas);
      return template;
    }

    public DbConnection CriarConexao(string servidor = null)
    {
      var configuracao = string.IsNullOrEmpty(servidor)
        ? this.stringDeConexao : this.template.Replace("{servidor}", servidor);

      return new Npgsql.NpgsqlConnection(configuracao);
    }
  }
}
