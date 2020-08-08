using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using Director.Configuracoes;
using Keep.Paper.Api;
using Keep.Paper.Services;
using Keep.Tools;
using Keep.Tools.Collections;
using Keep.Tools.Sequel;
using Keep.Tools.Sequel.Runner;

namespace Director.Conectores
{
  public class DbPdv
  {
    private readonly ICommonSettings settings;

    private string _stringDeConexao;
    private string _template;

    private const string TemplatePadrao =
      "Server=;Database=DBPDV;User ID=postgres;Password=local";

    public DbPdv(ICommonSettings settings)
    {
      this.settings = settings;
    }

    public DbConnection CriarConexao()
    {
      var stringDeConexao = ObterStringDeConexao();
      return new Npgsql.NpgsqlConnection(stringDeConexao);
    }

    public DbConnection CriarConexao(string servidor, int porta = 5432,
      string banco = "DBpdv")
    {
      var stringDeConexaoOriginal = ObterStringDeConexao();
      var template = ObterTemplate(stringDeConexaoOriginal);

      var stringDeConexao = template
        .Replace("{servidor}", servidor)
        .Replace("{porta}", porta.ToString())
        .Replace("{banco}", banco);

      return new Npgsql.NpgsqlConnection(stringDeConexao);
    }

    private string ObterStringDeConexao()
    {
      if (_stringDeConexao == null)
      {
        var stringDeConexao = settings.Get(Chaves.StringsDeConexao.Pdv);
        _stringDeConexao = stringDeConexao ?? TemplatePadrao;
      }
      return _stringDeConexao;
    }

    private string ObterTemplate(string stringDeConexao)
    {
      if (_template == null)
      {
        var entradas =
          from parametro in stringDeConexao.Split(";")
          let partes = parametro.Split('=')
          let chave = partes.First()
          let valorBase = string.Join("=", partes.Skip(1))
          let valor = CriarValorDeTemplate(chave, valorBase)
          select $"{chave}={valor}";

        _template = string.Join(";", entradas);
      }
      return _template;
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
  }
}
