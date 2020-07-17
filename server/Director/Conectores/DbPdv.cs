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
        let valor = CriarValorDeTemplate(chave, valorBase)
        select $"{chave}={valor}";

      var template = string.Join(";", entradas);
      return template;
    }

    private string CriarValorDeTemplate(string chave, string valorBase)
    {
      switch (chave.ToLower())
      {
        case "server":
        case "data source":
        case "host":
          return "{servidor}";

        case "port":
          return "{porta}";

        case "database":
        case "default catalog":
          return "{banco}";

        default:
          return valorBase;
      }
    }

    public DbConnection CriarConexao()
    {
      return new Npgsql.NpgsqlConnection(this.stringDeConexao);
    }

    public DbConnection CriarConexao(string servidor, int porta = 5432,
      string banco = "DBpdv")
    {
      var stringDeConexao = this.template
        .Replace("{servidor}", servidor)
        .Replace("{porta}", porta.ToString())
        .Replace("{banco}", banco);

      return new Npgsql.NpgsqlConnection(stringDeConexao);
    }
  }
}
