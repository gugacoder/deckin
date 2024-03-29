﻿using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Keep.Tools.Sequel;
using Keep.Tools.Sequel.Runner;

namespace Mercadologic.Dominio.Concentrador
{
  /// <summary>
  /// Procedure de preparação da base do Concentrador para receber a carga.
  /// A procedure atualizada informações de promoção e retorna a lista de
  /// tabelas que devem ser exportadas. Para cada tabela existe uma procedure
  /// correspondente na forma: exportar_carga_automatica_TABELA
  /// </summary>
  public static class preparar_carga_automatica
  {
    /// <summary>
    /// Procedure de preparação da base do Concentrador para receber a carga.
    /// A procedure atualizada informações de promoção e retorna a lista de
    /// tabelas que devem ser exportadas. Para cada tabela existe uma procedure
    /// correspondente na forma: exportar_carga_automatica_TABELA
    /// </summary>
    public static async Task<string[]> ExecutarAsync(DbConnection cnConcentrador,
      CancellationToken stopToken)
    {
      var tabelas = await
        $"select preparar_carga_automatica ()"
          .AsSql()
          .SelectAsync<string>(cnConcentrador, stopToken: stopToken);
      return tabelas;
    }
  }
}