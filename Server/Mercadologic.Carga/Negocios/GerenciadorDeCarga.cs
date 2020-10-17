using System;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AppSuite.Conectores;
using Keep.Paper;
using Keep.Paper.Api;
using Keep.Tools.Collections;
using Mercadologic.Carga.Dominio.Director.mlogic;
using Mercadologic.Carga.Negocios.Algoritmos;
using Mercadologic.Dominio.Director.dbo;

namespace Mercadologic.Carga.Negocios
{
  public class GerenciadorDeCarga
  {
    private readonly IServiceProvider provider;
    private readonly DbDirector dbDirector;
    private readonly DbConcentrador dbConcentrador;
    private readonly IAudit audit;

    public GerenciadorDeCarga(IServiceProvider provider,
      DbDirector dbDirector, DbConcentrador dbConcentrador, IAudit audit)
    {
      this.provider = provider;
      this.dbDirector = dbDirector;
      this.dbConcentrador = dbConcentrador;
      this.audit = audit;
    }

    public async Task PublicarCargas(CancellationToken stopToken)
    {
      // Como produzir um pacote ZIP contendo a carga para uma empresa?
      //  1.  Importe os dados do DIRECTOR para a base do CONCENTRADOR dessa
      //      empresa.
      //  2.  A partir da base do CONCENTRADOR produza uma pasta temporária
      //      contendo os arquivos de CARGA exportados para TXT (ou CSV).
      //  3.  Compacte esta pasta temporária com ZIP.
      //  4.  Nome o arquivo GZIP com o número de identificação do pacote e
      //      sua versão:
      //        carga-v{VERSAO}-e{LOJA}.zip
      //      Exemplos:
      //        carga-v9023-e4.zip
      //        carga-v9023-e11.zip

      using var cnDirector = await dbDirector.ConnectAsync(stopToken);

      var empresas = await TBempresa_mercadologic.ObterAsync(
        cnDirector, stopToken);

      var cargas = await TBcarga_publicada.ObterProximasCargasAsync(
        cnDirector, empresas.ToArray(x => x.DFcod_empresa), stopToken);

      var cargasPorEmpresa =
        from carga in cargas
        join empresa in empresas
          on carga.DFcod_empresa equals empresa.DFcod_empresa
        group carga by empresa into g
        select new
        {
          empresa = g.Key,
          cargas = g.ToArray()
        };

      await Task.WhenAll(
        cargasPorEmpresa
          .Where(x =>
            x.empresa.DFdata_inativacao == null &&
            x.empresa.DFip_bloqueado_ate == null)
          .Select(x =>
            PublicarCargaDaEmpresaNoDriveAsync(
              cnDirector, x.empresa, x.cargas, stopToken)));
    }

    private async Task PublicarCargaDaEmpresaNoDriveAsync(
      DbConnection cnDirector, TBempresa_mercadologic empresa,
      TBcarga_publicada[] cargas, CancellationToken stopToken)
    {
      using var cnConcentrador = await dbConcentrador.ConnectAsync(stopToken,
        server: empresa.DFservidor,
        database: empresa.DFdatabase,
        port: empresa.DFporta,
        username: empresa.DFusuario,
        password: empresa.DFsenha);

      var cargasEmJson =
        await ExportadorDeCargaDoDirectorParaJson.ExecutarAsync(
          cnDirector, empresa, cargas, stopToken);

      var versao =
        await ImportadorDeCargaJsonNoConcentrador.ExecutarAsync(
          cnConcentrador, empresa, cargas, cargasEmJson, stopToken);

      var pasta =
        await ExportadorDeCargaDoConcentradorParaCsv.ExecutarAsync(
          cnConcentrador, empresa, versao, stopToken);

      var pacote =
        await CompactadorDeArquivosDaCarga.ExecutarAsync(
          cnConcentrador, empresa, versao, pasta, stopToken);

      await PublicadorDeCargaNoDrive.ExecutarAsync(
        cnDirector, cnConcentrador, empresa, pacote, stopToken);
    }
  }
}
