//using System;
//using Keep.Paper.Api;
//using Microsoft.Extensions.Configuration;

//namespace Director.Configuracoes
//{
//  /// <summary>
//  /// Coleção das configurações de primeiro uso da aplicação.
//  /// Estas configurações são realizadas no primeiro acesso ao sistema via HTTP.
//  /// Em geral se configura a conexão com o Director e a partir de então o
//  /// sistema carrega as demais configurações da própria base de dados.
//  /// </summary>
//  public class ConfiguracoesDePrimeiroUso
//  {
//    private readonly IConfiguration configuration;
//    private readonly ICommonSettings commonSettings;

//    public ConfiguracoesDePrimeiroUso(IConfiguration configuration,
//      ICommonSettings commonSettings)
//    {
//      this.configuration = configuration;
//      this.commonSettings = commonSettings;
//      this.StringsDeConexao = new TStringsDeConexao(this);
//    }

//    public TStringsDeConexao StringsDeConexao { get; }

//    public bool Implantado
//    {
//      get => commonSettings.Get<bool>("Processa.App.Implantado");
//      set => commonSettings.Set("Processa.App.Implantado", value);
//    }

//    public class TStringsDeConexao
//    {
//      private readonly ConfiguracoesDePrimeiroUso host;

//      public TStringsDeConexao(ConfiguracoesDePrimeiroUso host)
//      {
//        this.host = host;
//      }

//      public string Director
//      {
//        get
//        {
//          var valor = host.configuration[
//            "Host:ConnectionStrings:Director:ConnectionString"];

//          if (valor == null)
//          {
//            valor = host.commonSettings.Get<string>(
//              "Processa.App.StringsDeConexao.Director");
//          }

//          return valor;
//        }
//        set => host.commonSettings.Set(
//          "Processa.App.StringsDeConexao.Director", value);
//      }
//    }
//  }
//}
