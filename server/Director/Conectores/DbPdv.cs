using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using Keep.Paper.Api;
using Keep.Paper.Services;
using Keep.Tools;
using Keep.Tools.Collections;
using Keep.Tools.Sequel;
using Keep.Tools.Sequel.Runner;
using Microsoft.Extensions.Configuration;

namespace Director.Conectores
{
  public class DbPdv
  {
    private string _stringDeConexao;
    private string _template;

    private const string TemplatePadrao =
      "Server=;Database=DBPDV;User ID=postgres;Password=local;";

    private readonly IConfiguration configuration;

    public DbPdv(IConfiguration configuration)
    {
      this.configuration = configuration;
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
        var stringDeConexao = configuration[
          "ConnectionStrings:Pdv:ConnectionString"];

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
          where !string.IsNullOrEmpty(chave)
          where !string.IsNullOrEmpty(valor)
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
