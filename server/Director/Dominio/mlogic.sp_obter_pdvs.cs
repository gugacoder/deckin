using System;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using Keep.Tools.Sequel;
using Keep.Tools.Sequel.Runner;

namespace Director.Dominio.mlogic
{
  /// <summary>
  /// Mapeamento do resultado da procedure: mlogic.sp_obter_pdvs
  /// </summary>
  public partial class sp_obter_pdvs
  {
    public int DFcod_empresa { get; set; }
    public int DFid_pdv { get; set; }
    public string DFdescricao { get; set; }
    public string DFip { get; set; }
    public string DFbanco_dados { get; set; }
    public DateTime? DFdesativado { get; set; }
    public DateTime? DFreplicacao_desativado { get; set; }
    public int DFatualizacao { get; set; }

    #region Métodos de apoio

    /// <summary>
    /// Obtém informações sobre os PDVs de uma empresa indicada.
    /// </summary>
    /// <param name="connection">Uma conexão ativa com o DBdirector.</param>
    /// <param name="DFcod_empresa">O código da empresa pesquisada.</param>
    /// <param name="stopToken">Um TOKEN para cancelamento assíncrono da tareafa.</param>
    /// <returns></returns>
    public static async Task<sp_obter_pdvs[]> ObterAsync(DbConnection connection,
      int DFcod_empresa, CancellationToken stopToken = default)
    {
      var pdvs = await
        @"exec mlogic.sp_obter_pdvs @DFcod_empresa"
          .AsSql()
          .Set(new { DFcod_empresa })
          .SelectAsync<sp_obter_pdvs>(connection, stopToken: stopToken);
      return pdvs;
    }

    #endregion
  }
}
