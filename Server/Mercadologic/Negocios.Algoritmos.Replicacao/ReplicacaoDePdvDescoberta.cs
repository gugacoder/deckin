using System;
using System.Data.Common;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AppSuite.Conectores;
using Keep.Paper.Api;
using Keep.Tools;
using Keep.Tools.Collections;
using Keep.Tools.Sequel;
using Keep.Tools.Sequel.Runner;
using Mercadologic.Dominio.Director.dbo;
using Mercadologic.Dominio.Director.mlogic;

namespace Mercadologic.Negocios.Algoritmos.Replicacao
{
  /// <summary>
  /// Algoritmo de descoberta dos PDVs.
  /// A descoberta é feita em duas etapas:
  /// 1.  Os parâmetros de conexão com os Concentradores do Mercadologic são
  ///     descobertos a partir da base do Director.
  /// 2.  Os Concentradores descobertos são consultados e os parâmetros de
  ///     conexão com seus PDVs cadastrados são descobertos.
  /// </summary>
  public class ReplicacaoDePdvDescoberta
  {
    private readonly DbDirector dbDirector;
    private readonly DbConcentrador dbConcentrador;
    private readonly IAudit audit;
    private readonly ReplicacaoDePdvFalhas falhas;

    public ReplicacaoDePdvDescoberta(DbDirector dbDirector,
      DbConcentrador dbConcentrador, IAudit audit)
    {
      this.dbDirector = dbDirector;
      this.dbConcentrador = dbConcentrador;
      this.audit = audit;
      this.falhas = new ReplicacaoDePdvFalhas(dbDirector, audit);
    }

    public async Task<TBpdv[]> ObterPdvsAsync(
      CancellationToken stopToken = default)
    {
      return await LocalizarPdvsAsync((TBempresa_mercadologic[])null, stopToken);
    }

    public async Task<TBpdv[]> ObterPdvsAsync(TBempresa_mercadologic empresa,
      CancellationToken stopToken = default)
    {
      return (empresa == null)
        ? await Task.FromResult(new TBpdv[0])
        : await LocalizarPdvsAsync(new[] { empresa }, stopToken);
    }

    public async Task<TBpdv[]> ObterPdvsAsync(
      TBempresa_mercadologic[] empresas,
      CancellationToken stopToken = default)
    {
      return await LocalizarPdvsAsync(empresas, stopToken);
    }

    private async Task<TBpdv[]> LocalizarPdvsAsync(
      TBempresa_mercadologic[] empresas = default,
      CancellationToken stopToken = default)
    {
      try
      {
        empresas = await LocalizarEmpresasAsync(empresas, stopToken);

        // Os Concentradores serão pesquisados em paralelo.
        // Para cada Concentrador será obtido sua lista de PDVs cadastrados.
        var tarefas = empresas.Select(empresa =>
          LocalizarPdvsDaEmpresaAsync(empresa, stopToken));

        var pdvsPorEmpresa = await Task.WhenAll(tarefas);
        var pdvs = pdvsPorEmpresa.SelectMany().ToArray();

        return pdvs;
      }
      catch (Exception ex)
      {
        audit.LogDanger(
          To.Text(
            "Falhou a tentativa de obter informação sobre os PDVs.",
            ex),
          GetType());

        await falhas.ReportarAsync(new TBfalha_replicacao
        {
          DFevento = TBfalha_replicacao.EventoLocalizarPdvs,
          DFfalha = ex.GetCauseMessage(),
          DFfalha_detalhada = To.Text(ex)
        }, stopToken);

        return await Task.FromResult(new TBpdv[0]);
      }
    }

    private async Task<TBempresa_mercadologic[]> LocalizarEmpresasAsync(
      TBempresa_mercadologic[] filtro, CancellationToken stopToken)
    {
      try
      {
        using var cnDirector = await dbDirector.ConnectAsync(stopToken);

        var empresas = filtro;
        if (empresas?.Any() != true)
        {
          empresas = await TBempresa_mercadologic.ObterAsync(cnDirector, stopToken);
        }

#if DEBUG
        // Apenas para depuração, bases com * serão ignoradas.
        // Apenas um método de escolha do caso em estudo.
        empresas = empresas.Where(x => !x.DFdatabase.Contains("*")).ToArray();
#endif

        return empresas;
      }
      catch (Exception ex)
      {
        audit.LogDanger(
          To.Text(
            "Falhou a tentativa de obter informação sobre Concentradores.",
            ex),
          GetType());

        await falhas.ReportarAsync(new TBfalha_replicacao
        {
          DFevento = TBfalha_replicacao.EventoLocalizarPdvs,
          DFfalha = ex.GetCauseMessage(),
          DFfalha_detalhada = To.Text(ex)
        }, stopToken);

        return await Task.FromResult(new TBempresa_mercadologic[0]);
      }
    }

    private async Task<TBpdv[]> LocalizarPdvsDaEmpresaAsync(
      TBempresa_mercadologic empresa, CancellationToken stopToken)
    {
      try
      {
        using var cnDirector = await dbDirector.ConnectAsync(stopToken);

        try
        {
          await RegistrarAcessoAoIpAsync(cnDirector, empresa.DFservidor, stopToken);

          // Estabelecendo uma conexão com o Concetrador do Mercadologic
          //
          using var cnConcentrador = await dbConcentrador.ConnectAsync(stopToken,
            server: empresa.DFservidor,
            database: empresa.DFdatabase,
            port: empresa.DFporta,
            username: empresa.DFusuario,
            password: empresa.DFsenha);

          var pdvs = await TBpdv.ObterAsync(cnConcentrador, stopToken);
          return pdvs;
        }
        catch (Exception ex)
        {
          await BloquearAcessoAoIpAsync(cnDirector, empresa.DFservidor, stopToken);

          await falhas.ReportarAsync(new TBfalha_replicacao
          {
            DFevento = TBfalha_replicacao.EventoLocalizarPdvs,
            DFfalha = ex.GetCauseMessage(),
            DFfalha_detalhada = To.Text(ex),
            DFcod_empresa = empresa.DFcod_empresa
          }, stopToken);
        }

      }
      catch (Exception ex)
      {
        audit.LogDanger(
          To.Text(
            $"Falhou a tentativa de obter informação sobre os PDVs da empresa {empresa.DFcod_empresa}.",
            ex),
          GetType());
      }

      return await Task.FromResult(new TBpdv[0]);
    }

    /// <summary>
    // Registra a tentativa de conexão com o Concentrador.
    /// </summary>
    private async Task RegistrarAcessoAoIpAsync(DbConnection cnDirector,
      string ip, CancellationToken stopToken)
    {
      using var tx = await cnDirector.BeginTransactionAsync(stopToken);

      var ret = await
        @"merge mlogic.TBacesso_mercadologic as atual
          using (select * from(values(@ip)) as T(DFip)) as novo
             on atual.DFip = novo.DFip
           when not matched then
                insert(DFip, DFultimo_acesso)
                values(novo.DFip, current_timestamp)
           when matched then
                update set atual.DFultimo_acesso = current_timestamp
                         , atual.DFbloqueado_ate = null
                         , atual.DFrazao_bloqueio = null;"
          .AsSql()
          .Set(new { ip })
          .TryExecuteAsync(cnDirector, tx, stopToken);

      if (ret.Ok) await tx.CommitAsync();
    }

    /// <summary>
    /// Bloqueia o IP do PDV por um tempo para evitar falhas sucessivas de
    /// conexão.
    /// </summary>
    private async Task BloquearAcessoAoIpAsync(DbConnection cnDirector,
      string ip, CancellationToken stopToken)
    {
      using var tx = await cnDirector.BeginTransactionAsync(stopToken);

      var ret = await
        @"update mlogic.TBacesso_mercadologic
             set DFbloqueado_ate = dateadd(mi, @minutos, current_timestamp)
               , DFrazao_bloqueio = error_message()
           where DFip = @ip"
          .AsSql()
          .Set(new { ip, minutos = 10 })
          .TryExecuteAsync(cnDirector, tx, stopToken);

      if (ret.Ok) await tx.CommitAsync();
    }
  }
}
