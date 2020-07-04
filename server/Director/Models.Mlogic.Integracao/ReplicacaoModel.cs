using System;
using System.Linq;
using System.Threading.Tasks;
using Director.Adaptadores;
using Director.Conectores;
using Keep.Tools;
using static Director.Adaptadores.DirectorAudit;

namespace Director.Modelos.Mlogic.Integracao
{
  public class ReplicacaoModel
  {
    private readonly DbDirector dbDirector;
    private readonly DbPdv dbPdv;
    private readonly DirectorAudit audit;

    public ReplicacaoModel(DbDirector dbDirector, DbPdv dbPdv, DirectorAudit audit)
    {
      this.dbDirector = dbDirector;
      this.dbPdv = dbPdv;
      this.audit = audit;
    }

    public async Task ReplicarPdvsAsync()
    {
      var pdvs = await dbPdv.GetIpsAsync();

      audit.LogInfo(
        Join.Lines(
          "Iniciando procedimento de replicação dos PDVs",
          string.Join(@"\n- ", pdvs)
        ),
        GetType()
      );

      var tarefas = pdvs.Select(ReplicarPdvAsync).ToArray();
      await Task.WhenAll(tarefas);
    }

    public async Task ReplicarPdvAsync(string pdv)
    {
      try
      {
        audit.LogInfo(
          Join.Lines(
            $"Iniciando procedimento de replicação do PDV: {pdv}"
          ),
          GetType()
        );

        using var cn = dbPdv.GetConexao(pdv);

        await cn.OpenAsync();

      }
      catch (Exception ex)
      {
        audit.LogSuccess(
          Join.Lines(
            $"PDV replicado com falhas: {pdv}",
            ex
          ),
          GetType()
        );
      }
    }
  }
}
