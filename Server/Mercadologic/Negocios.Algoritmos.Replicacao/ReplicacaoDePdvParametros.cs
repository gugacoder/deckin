using System;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using Keep.Tools.Sequel;
using Keep.Tools.Sequel.Runner;

namespace Mercadologic.Negocios.Algoritmos.Replicacao
{
  public class ReplicacaoDePdvParametros
  {
    private THistorico _historico;

    public bool Ativado { get; set; }

    public THistorico Historico
    {
      get => _historico ??= new THistorico();
      set => _historico = value;
    }

    public class THistorico
    {
      public bool Ativado { get; set; }
      public int Dias { get; set; }
    }

    public static async Task<ReplicacaoDePdvParametros> ObterAsync(
      DbConnection cnDirector, CancellationToken stopToken)
    {
      try
      {
        var parametros = await
          @"select (select DFvalor
                     from mlogic.TBsis_config
                     where DFchave = 'replicacao.ativado'
                   ) as DFreplicacao
                 , (select DFvalor
                     from mlogic.TBsis_config
                     where DFchave = 'replicacao.historico.ativado'
                   ) as DFhistorico
                 , (select DFvalor
                     from mlogic.TBsis_config
                     where DFchave = 'replicacao.historico.dias'
                   ) as DFdias"
            .AsSql()
            .SelectOneAsync(cnDirector,
              (bool replicacao, bool historico, int dias) =>
              {
                var parametros = new ReplicacaoDePdvParametros();
                parametros.Ativado = replicacao;
                parametros.Historico.Ativado = historico;
                parametros.Historico.Dias = dias;
                return parametros;
              },
              stopToken: stopToken);

        return parametros;
      }
      catch (Exception ex)
      {
        throw new Exception(
          "Obtenção dos parâmetros do sistema a partir da base do Director concluída com falhas."
          , ex);
      }
    }
  }
}
