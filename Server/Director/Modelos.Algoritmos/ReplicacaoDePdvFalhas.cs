using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Director.Conectores;
using Director.Dominio.mlogic;
using Keep.Paper.Api;
using Keep.Tools;
using Keep.Tools.Sequel;
using Keep.Tools.Sequel.Runner;

namespace Director.Modelos.Algoritmos
{
  /// <summary>
  /// Utilitário para registro de falhas de replicação de dados do PDV para o
  /// Director.
  /// </summary>
  public class ReplicacaoDePdvFalhas
  {
    private DbDirector dbDirector;
    private IAudit audit;

    private readonly object @lock = new object();

    public ReplicacaoDePdvFalhas(DbDirector dbDirector, IAudit audit)
    {
      this.dbDirector = dbDirector;
      this.audit = audit;
    }

    public async Task ReportarAsync(TBfalha_replicacao falha,
      CancellationToken stopToken = default,
      [CallerMemberName] string chamador = null,
      [CallerLineNumber] int? linhaDoChamador = null,
      [CallerArgumentExpression("falha")] string argumentoDoChamador = null,
      [CallerFilePath] string arquivoDoChamador = null)
    {
      try
      {
        falha.DFevento ??= "falha";

        falha.DFfalha_detalhada =
          $"{chamador}:{linhaDoChamador}({argumentoDoChamador})\n" +
          $"-> {arquivoDoChamador}\n" +
          $"{falha.DFfalha_detalhada}";

        // O método estabelece sua própria conexão para garantir a gravação da
        // auditoria independentemente do que ocorrer no método que o chamou.
        using var cnConexao = await dbDirector.ConnectAsync(stopToken);

        using (var tx = await cnConexao.BeginTransactionAsync())
        {
          await
            @"insert into mlogic.TBfalha_replicacao (
                  DFevento
                , DFfalha
                , DFfalha_detalhada
                , DFcod_empresa
                , DFcod_pdv
                , DFtabela
              ) values (
                  @DFevento
                , @DFfalha
                , @DFfalha_detalhada
                , @DFcod_empresa
                , @DFcod_pdv
                , @DFtabela
              )"
            .AsSql()
            .Set(falha)
            .ExecuteAsync(cnConexao, tx);

          await tx.CommitAsync();
        }

        using (var tx = await cnConexao.BeginTransactionAsync())
        {
          await
            @"; with expiracao as (
                select DFevento
                     , max(DFid_falha_replicacao)-@HistoricoMaximo as DFmin_id_falha_replicacao
                  from mlogic.TBfalha_replicacao with (nolock)
                 group by DFevento
              )
              delete mlogic.TBfalha_replicacao
                from expiracao
               inner join mlogic.TBfalha_replicacao
                       on mlogic.TBfalha_replicacao.DFevento = expiracao.DFevento
                      and mlogic.TBfalha_replicacao.DFid_falha_replicacao < expiracao.DFmin_id_falha_replicacao"
            .AsSql()
            .Set(new { HistoricoMaximo = 10000 })
            .ExecuteAsync(cnConexao, tx);

          await tx.CommitAsync();
        }
      }
      catch (Exception ex)
      {
        audit.LogDanger(To.Text(
          "Falhou a tentativa de salvar no banco uma informação de auditoria."
          , ex)
          , GetType());
      }
    }
  }
}
