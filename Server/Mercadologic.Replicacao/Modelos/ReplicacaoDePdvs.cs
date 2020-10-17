using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AppSuite.Adaptadores;
using AppSuite.Conectores;
using Mercadologic.Replicacao.Dominio.dbo;
using Mercadologic.Replicacao.Dominio.mlogic;
using Mercadologic.Replicacao.Modelos.Algoritmos;
using Keep.Paper.Api;
using Keep.Tools;
using Keep.Tools.Collections;
using Keep.Tools.Sequel;
using Keep.Tools.Sequel.Runner;

namespace Mercadologic.Replicacao.Modelos
{
  public class ReplicacaoDePdvs
  {
    private readonly DbDirector dbDirector;
    private readonly DbConcentrador dbConcentrador;
    private readonly DbPdv dbPdv;
    private readonly IAudit audit;
    private readonly ReplicacaoDePdvFalhas falhas;

    public ReplicacaoDePdvs(DbDirector dbDirector,
      DbConcentrador dbConcentrador, DbPdv dbPdv, IAudit audit)
    {
      this.dbDirector = dbDirector;
      this.dbConcentrador = dbConcentrador;
      this.dbPdv = dbPdv;
      this.audit = audit;
      this.falhas = new ReplicacaoDePdvFalhas(dbDirector, audit);
    }

    private async Task<ReplicacaoDePdvParametros> ObterParametrosAsync(
      CancellationToken stopToken)
    {
      using var cnDirector = await dbDirector.ConnectAsync(stopToken);
      return await ReplicacaoDePdvParametros.ObterAsync(cnDirector, stopToken);
    }

    public async Task ApagarHistoricoAsync(CancellationToken stopToken)
    {
      var parametros = await ObterParametrosAsync(stopToken);
      if (!parametros.Ativado || !parametros.Historico.Ativado)
        return;

      var historico = new ReplicacaoDePdvHistorico(dbDirector, audit);
      await historico.ApagarHistoricoAsync(parametros.Historico.Dias, stopToken);
    }

    public async Task ReplicarPdvsAsync(CancellationToken stopToken)
    {
      try
      {
        var parametros = await ObterParametrosAsync(stopToken);
        if (!parametros.Ativado)
          return;

        var descoberta =
          new ReplicacaoDePdvDescoberta(dbDirector, dbConcentrador, audit);
        var replicacao =
          new ReplicacaoDePdv(dbDirector, dbPdv, audit);

        var pdvs = await descoberta.ObterPdvsAsync(stopToken);

        var pdvsAtivos = pdvs
          .Where(pdv =>
            pdv.DFdesativado == null && pdv.DFreplicacao_desativado == null)
          .ToArray();

        var tarefas = pdvsAtivos.Select(pdv =>
          replicacao.ReplicarPdvAsync(pdv, stopToken));

        await Task.WhenAll(tarefas);
      }
      catch (Exception ex)
      {
        audit.LogDanger(
          To.Text(
            "Falhou a tentativa de replicar PDVs.",
            ex),
          GetType());

        await falhas.ReportarAsync(new TBfalha_replicacao
        {
          DFevento = TBfalha_replicacao.EventoReplicar,
          DFfalha = ex.GetCauseMessage(),
          DFfalha_detalhada = To.Text(ex)
        }, stopToken);
      }
    }
  }
}
