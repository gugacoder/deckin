using System;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using Keep.Tools.Sequel;
using Keep.Tools.Sequel.Runner;

namespace Director.Dominio.dbo
{
  /// <summary>
  /// Mapeamento de campos da tabela de empresa.
  /// </summary>
  public class TBempresa
  {
    // Apenas os campos relevantes até o momento.
    // Outros campos podem ser mapeados sem prejuizo para os algoritmos atuais.

    public int DFcod_empresa { get; set; }
    public DateTime? DFdata_inativacao { get; set; }

    /// <summary>
    /// Obtém informações sobre as empresas cadastradas.
    /// </summary>
    /// <param name="connection">Uma conexão ativa com o DBdirector.</param>
    /// <param name="stopToken">Um TOKEN para cancelamento assíncrono da tareafa.</param>
    /// <returns></returns>
    public static async Task<TBempresa[]> ObterAsync(DbConnection connection,
      CancellationToken stopToken)
    {
      var pdvs = await
        @"select DFcod_empresa
               , DFdata_inativacao
            from TBempresa"
          .AsSql()
          .SelectAsync<TBempresa>(connection, stopToken: stopToken);
      return pdvs;
    }
  }
}
