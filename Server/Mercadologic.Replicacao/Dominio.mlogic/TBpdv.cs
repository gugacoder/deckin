using System;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using Keep.Tools.Sequel;
using Keep.Tools.Sequel.Runner;

namespace Mercadologic.Replicacao.Dominio.mlogic
{
  /// <summary>
  /// Mapeamento do resultado da procedure: mlogic.sp_obter_pdvs
  /// </summary>
  public partial class TBpdv
  {
    public int DFcod_empresa { get; set; }
    public int DFcod_pdv { get; set; }
    public string DFdescricao { get; set; }
    public string DFip { get; set; }
    public string DFbanco_dados { get; set; }
    public DateTime? DFdesativado { get; set; }
    public DateTime? DFreplicacao_desativado { get; set; }
    public int DFatualizacao { get; set; }

    [Obsolete("Mantido apenas para compatibilidade com versões anteriores. Prefira usar DFcod_pdv.")]
    public int DFid_pdv
    {
      get => DFcod_pdv;
      set => DFcod_pdv = value;
    }

    /// <summary>
    /// Obtém informações sobre os PDVs cadastrados na base de um Concentrador
    /// do Mercadologic.
    /// </summary>
    /// <param name="cnConcentrador">
    /// Uma conexão ativa com o DBMercadologic de um Concentrador do Mercadologic.
    /// </param>
    /// <param name="stopToken">Um TOKEN para cancelamento assíncrono da tareafa.</param>
    /// <returns></returns>
    public static async Task<TBpdv[]> ObterAsync(
      DbConnection cnConcentrador, CancellationToken stopToken)
    {
      var pdvs = await
        @"select (select id from empresa
                  where desativado is null
                  limit 1)                 as ""DFcod_empresa""
               , pdv.id                    as ""DFcod_pdv""
               , pdv.identificador         as ""DFdescricao""
               , pdv.ip                    as ""DFip""
               , pdv.banco_dados           as ""DFbanco_dados""
               , pdv.desativado            as ""DFdesativado""
               , pdv.replicacao_desativado as ""DFreplicacao_desativado""
               , pdv.atualizacao           as ""DFatualizacao""
           from pdv"
          .AsSql()
          .SelectAsync<TBpdv>(cnConcentrador, stopToken: stopToken);
      return pdvs;
    }
  }
}
