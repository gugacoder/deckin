using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Director.Conectores;
using Director.Dominio.Mlogic.Integracao;
using Keep.Paper.Api;
using Keep.Tools;
using Keep.Tools.Sequel;
using Keep.Tools.Sequel.Runner;

namespace Director.Modelos.Mlogic.Integracao
{
  public class LocalizadorDePdv
  {
    private readonly DbDirector dbDirector;
    private readonly DbPdv dbPdv;
    private readonly IAudit audit;

    public LocalizadorDePdv(DbDirector dbDirector, DbPdv dbPdv, IAudit audit)
    {
      this.dbDirector = dbDirector;
      this.dbPdv = dbPdv;
      this.audit = audit;
    }

    public async Task<Pdv[]> ObterPdvsAsync(CancellationToken stopToken)
    {
      try
      {
        audit.Log(
          "Obtendo os PDVs a partir da base do Director...",
          GetType());

        using var cnDirector = dbDirector.CriarConexao();
        await cnDirector.OpenAsync(stopToken);

        var pdvs = await
         $@"select DFid_pdv as {nameof(Pdv.IdPdv)}
                 , DFcod_empresa as {nameof(Pdv.IdEmpresa)}
                 , DFdescricao as {nameof(Pdv.Descricao)}
                 , DFendereco_rede as {nameof(Pdv.EnderecoDeRede)}
                 , DFativado as {nameof(Pdv.Ativado)}
                 , DFreplicacao_ativado as {nameof(Pdv.ReplicacaoAtivado)}
              from mlogic.vw_pdv
             where DFativado = 1"
            .AsSql()
            .SelectAsync<Pdv>(cnDirector, stopToken: stopToken);

        audit.Log(
          Join.Lines(
            "Obtenção dos PDVs a partir da base do Director concluída:",
            pdvs.Length > 0
              ? (object)pdvs.Select(pdv => pdv.EnderecoDeRede)
              : (object)"(Nenhum IP de PDV encontrado)"),
          GetType());

        return pdvs;
      }
      catch (Exception ex)
      {
        audit.LogDanger(
          Join.Lines(
            "Obtenção dos PDVs a partir da base do Director concluída com falhas.",
            ex),
          GetType());
        return new Pdv[0];
      }
    }
  }
}
