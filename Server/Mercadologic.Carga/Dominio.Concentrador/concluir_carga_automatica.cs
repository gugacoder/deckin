using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Keep.Tools.Sequel;
using Keep.Tools.Sequel.Runner;
using Mercadologic.Carga.Tipos;

namespace Mercadologic.Carga.Dominio.Concentrador
{
  /// <summary>
  /// Procedure de conclusão da carga automática.
  /// A procedure encerra a carga na base do Mercadologic e retorna
  /// o ID da carga gerada e sua data de atualização.
  /// </summary>
  public static class concluir_carga_automatica
  {
    /// <summary>
    /// Procedure de conclusão da carga automática.
    /// A procedure encerra a carga na base do Mercadologic e retorna
    /// o ID da carga gerada e sua data de atualização.
    /// </summary>
    /// <returns>O ID da carga gerada e a data da sua atualização.</returns>
    public static async Task<Versao> ExecutarAsync(DbConnection cnConcentrador,
      CancellationToken stopToken)
    {
      var versao = await
        @"select id_carga as Numero
               , descricao_carga as Descricao
            from concluir_carga_automatica ()"
          .AsSql()
          .SelectOneAsync<Versao>(cnConcentrador, stopToken: stopToken);
      return versao;
    }
  }
}