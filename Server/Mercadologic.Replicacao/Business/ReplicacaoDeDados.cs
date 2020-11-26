using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;
using Keep.Paper.Api;
using Microsoft.Extensions.Configuration;
using Keep.Paper.Databases;
using Mercadologic.Replicacao.Databases;
using Keep.Tools.Sequel;
using Keep.Tools.Sequel.Runner;
using System.Diagnostics;
using Keep.Tools;
using Keep.Tools.Collections;

namespace Mercadologic.Replicacao.Business
{
  public class ReplicacaoDeDados
  {
    private readonly IDbConnector<DbMercadologic> dbMercadologic;
    private readonly IAudit<ReplicacaoDeDados> audit;

    public ReplicacaoDeDados(IDbConnector<DbMercadologic> dbMercadologic,
      IAudit<ReplicacaoDeDados> audit)
    {
      this.dbMercadologic = dbMercadologic;
      this.audit = audit;
    }

    public async Task<ICollection<int>> ObterEmpresasReplicaveisAsync(
      CancellationToken stopToken)
    {
      using var cnMercadologic = await dbMercadologic.ConnectAsync(stopToken);

      var empresas = await
        @"select DFcod_empresa
            from replica.vw_empresa with (nolock)
           where DFreplicacao_desativado is null"
          .AsSql()
          .SelectAsync<int>(cnMercadologic, stopToken: stopToken);

      return empresas;
    }

    public async Task ReplicarDadosDaEmpresaAsync(int codEmpresa,
      CancellationToken stopToken)
    {
      using var cnMercadologic = await dbMercadologic.ConnectAsync(stopToken);

      AuditMessages(cnMercadologic as SqlConnection, codEmpresa);

      await
        @"exec replica.replicar_mercadologic @codEmpresa"
          .AsSql()
          .Set(new { codEmpresa })
          .ExecuteAsync(cnMercadologic, stopToken: stopToken);

      await Task.Yield();
    }

    private void AuditMessages(SqlConnection cn, int codEmpresa)
    {
      if (cn == null)
        return;

      cn.FireInfoMessageEventOnUserErrors = true;
      cn.InfoMessage += (o, e) =>
      {
        if (!string.IsNullOrEmpty(e.Message))
        {
          audit.LogTrace(e.Message);
          Debug.WriteLine($"[Trace] Empresa {codEmpresa}: {e.Message}");
        }

        foreach (SqlError error in e.Errors)
        {
          var level = error.Class >= 16 ? Level.Danger : Level.Trace;

          audit.Log(level,
            To.Text(
              $"Empresa {codEmpresa}: {error.Message}",
              $"- Em {error.Procedure} linha {error.LineNumber}"
              ));

          Debug.WriteLine(
            To.Text(
              $"[{level}] Empresa {codEmpresa}: {error.Message}",
              $"- Em {error.Procedure} linha {error.LineNumber}"
              ));
        }
      };
    }
  }
}
