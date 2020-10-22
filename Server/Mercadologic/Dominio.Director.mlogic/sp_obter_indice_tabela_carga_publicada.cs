using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Keep.Tools.Sequel;
using Keep.Tools.Sequel.Runner;

namespace Mercadologic.Dominio.Director.mlogic
{
  /// <summary>
  /// Método de planejamento da sequência de tabelas atualizadas com a carga.
  /// 
  /// Para cada tabela obtida existe uma procedure no Director sob o
  /// padrão de nome:
  /// -   mlogic.sp_carga_TABELA
  /// E uma procedure da base do Concentrador do Mercadologic sob o nome de:
  /// -   public.carga_TABELA
  /// 
  /// A ordem de tabelas obtida deve ser a ordem de execução das procedures.
  /// </summary>
  public static class sp_obter_indice_tabela_carga_publicada
  {
    /// <summary>
    /// Método de planejamento da sequência de tabelas atualizadas com a carga.
    /// 
    /// Para cada tabela obtida existe uma procedure no Director sob o
    /// padrão de nome:
    /// -   mlogic.sp_carga_TABELA
    /// E uma procedure da base do Concentrador do Mercadologic sob o nome de:
    /// -   public.carga_TABELA
    /// 
    /// A ordem de tabelas obtida deve ser a ordem de execução das procedures.
    /// </summary>
    /// <param name="idsDeCargaEmpresaPublicada">Ids das tabelas publicadas.</param>
    /// <returns>Coleção dos nomes de tabelas para integração.</returns>
    public static async Task<Tabela[]> ExecutarAsync(DbConnection cnDirector,
      int[] idsDeCargaPublicada, CancellationToken stopToken)
    {
      var tabelas = await
        @"exec mlogic.sp_obter_indice_tabela_carga_publicada @{idsDeCargaPublicada}"
          .AsSql()
          .Set(new { idsDeCargaPublicada })
          .SelectAsync<Tabela>(cnDirector, stopToken: stopToken);
      return tabelas;
    }

    /// <summary>
    /// Configuração de exportação de uma tabela.
    /// </summary>
    public class Tabela
    {
      /// <summary>
      /// Nome da tabela.
      /// </summary>
      public string DFtabela { get; set; }

      /// <summary>
      /// Destino determina onde o exportador deve salvar os dados.
      /// Os valores aceitos são:
      ///  -   (nulo)
      ///      -   O exportador executa a procedure de exportação mas ignora o resultado.
      ///          Útil quando a própria procedure se encarrega da exportação e portanto
      ///          não há nada mais para o exportador executar.
      ///  -   CONCENTRADOR
      ///      -   Exporta os dados para o algoritmo de exportação alternativo do Mercadologic.
      ///          Neste algoritmo os dados são salvos na base do Concentrador e depois
      ///          extraídos de lá e publicados no Drive.
      ///          Este algoritmo deixará de existir assim que os dados do Concentrador
      ///          estiverem disponíveis no Director tornando o Concentrador desnecessário.
      ///  -   drive://Mercadologic/...
      ///      -   (RESERVADO) Futuramente este caminho será usado para publicar arquivos
      ///          diretamente no Drive.O Drive é uma pasta virtual disponível via HTTP.
      ///          Geralmente hospedado em:
      ///              http://localhost:90/Drive/Mercadologic
      ///  -   file://...
      ///      -   (RESERVADO) Futuramente este caminho será usado para publicar arquivos
      ///          fisicamente no servidor ou em pastas da rede.
      /// </summary>
      public string DFdestino { get; set; }
    }
  }
}
