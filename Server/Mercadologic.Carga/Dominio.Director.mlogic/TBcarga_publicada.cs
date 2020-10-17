using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Keep.Tools.Sequel;
using Keep.Tools.Sequel.Runner;

namespace Mercadologic.Carga.Dominio.Director.mlogic
{
  public partial class TBcarga_publicada
  {
    /// <summary>
    /// Valor do campo DFstatus para carga pendente de publicação.
    /// </summary>
    public const string Aguardando = "A";
    /// <summary>
    /// Valor do campo DFstatus para carga em processo de publicação.
    /// </summary>
    public const string EmExecucao = "E";
    /// <summary>
    /// Valor do campo DFstatus para carga já publicada.
    /// </summary>
    public const string Publicado = "P";
    /// <summary>
    /// Valor do campo DFstatus para falha na publicação da carga.
    /// </summary>
    public const string Falha = "F";

    public int DFid_carga_empresa_publicada { get; set; }
    public int DFid_carga_publicada { get; set; }
    public int DFcod_empresa { get; set; }
    public string DFidentificador { get; set; }
    public int? DFid_carga_agendada { get; set; }
    public DateTime DFdata_cadastro { get; set; }
    public DateTime? DFdata_publicacao { get; set; }
    public string DFstatus { get; set; }
    public string DFfalha { get; set; }
    public string DFfalha_detalhada { get; set; }

    #region Métodos

    /// <summary>
    /// Obtém as cargas indicadas.
    /// </summary>
    /// <param name="ids">Os ids das cargas procuradas.</param>
    /// <returns>As cargas obtidas.</returns>
    public static async Task<TBcarga_publicada> ObterAsync(
      DbConnection cnDirector, int id, CancellationToken stopToken)
    {
      var resultado = await ObterAsync(cnDirector, new[] { id }, stopToken);
      return resultado.FirstOrDefault();
    }

    /// <summary>
    /// Obtém as cargas indicadas.
    /// </summary>
    /// <param name="ids">Os ids das cargas procuradas.</param>
    /// <returns>As cargas obtidas.</returns>
    public static async Task<TBcarga_publicada[]> ObterAsync(
      DbConnection cnDirector, int[] ids, CancellationToken stopToken)
    {
      var cargas = await
        @"select *
            from mlogic.TBcarga_publicada
           where DFid_carga_publicada matches @ids"
          .AsSql()
          .Set(new { ids })
          .SelectAsync<TBcarga_publicada>(cnDirector, stopToken: stopToken);
      return cargas;
    }

    /// <summary>
    /// Obtém os IDs de cargas de empresa publicadas pendentes.
    /// As cargas de empresa publicadas pendentes são aquelas que devem
    /// ser atualizadas nos PDVs.
    /// </summary>
    /// <param name="codigosDasEmpresas">
    /// Códigos das empresas interessadas.
    /// Se vazio, serão obtidas as cargas publicadas de todas as empresas.
    /// </param>
    /// <returns>Os IDs das cargas de empresa publicadas pendentes.</returns>
    public static async Task<TBcarga_publicada[]> ObterProximasCargasAsync(
      DbConnection cnDirector, int[] codigosDasEmpresas,
      CancellationToken stopToken)
    {
      var cargasPendentes = await
        @"select *
            from mlogic.TBcarga_publicada with (nolock)
           where DFstatus = 'A'
             and DFcod_empresa matches if set @codigosDasEmpresas"
        .AsSql()
        .Set(new { codigosDasEmpresas })
        .SelectAsync<TBcarga_publicada>(cnDirector, stopToken: stopToken);
      return cargasPendentes;
    }

    /// <summary>
    /// Salva na base de dados os campos alteráveis da carga.
    /// </summary>
    /// <param name="cargas">As cargas modificadas.</param>
    public static async Task SalvarCargaAsync(DbConnection cnDirector,
      TBcarga_publicada[] cargas, CancellationToken stopToken)
    {
      var valores = string.Join(",",
        cargas.Select(x =>
        {
          var dataPublicacao = (x.DFdata_publicacao != null)
            ? $"'{x.DFdata_publicacao?.ToString("yyyy-MM-dd HH:mm:ss")}'"
            : "null";
          var falha = (x.DFfalha != null)
            ? $"'{x.DFfalha.Replace("'", " ")}'"
            : "null";
          var falhaDetalhada = (x.DFfalha_detalhada != null)
            ? $"'{x.DFfalha_detalhada.Replace("'", " ")}'"
            : "null";

          var argumentos =
            string.Join(","
              , $"'{x.DFid_carga_publicada}'"
              , $"'{x.DFidentificador}'"
              , dataPublicacao
              , $"'{x.DFstatus}'"
              , falha
              , falhaDetalhada
            );

          return $"({argumentos})";
        })
      );

      using var tx = cnDirector.BeginTransaction();

      await
        @"; with TBdados as (
            select DFid_carga_publicada
                 , DFidentificador
                 , DFdata_publicacao
                 , DFstatus
                 , DFfalha
                 , DFfalha_detalhada
              from(values
                @{valores}
              ) as T(DFid_carga_publicada
                   , DFidentificador
                   , DFdata_publicacao
                   , DFstatus
                   , DFfalha
                   , DFfalha_detalhada
                   )
          )
          update mlogic.TBcarga_publicada
             set DFidentificador   = TBdados.DFidentificador
               , DFdata_publicacao = TBdados.DFdata_publicacao
               , DFstatus          = TBdados.DFstatus          
               , DFfalha           = TBdados.DFfalha          
               , DFfalha_detalhada = TBdados.DFfalha_detalhada
            from TBdados
           where TBdados.DFid_carga_publicada = mlogic.TBcarga_publicada.DFid_carga_publicada"
          .AsSql()
          .Set(new { valores })
          .ExecuteAsync(cnDirector, tx, stopToken);

      tx.Commit();
    }

    #endregion
  }
}
