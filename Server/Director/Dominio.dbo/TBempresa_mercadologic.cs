﻿using System;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using Keep.Tools.Sequel;
using Keep.Tools.Sequel.Runner;

namespace Director.Dominio.dbo
{
  public class TBempresa_mercadologic
  {
    public int DFcod_empresa { get; set; }
    public string DFnome_fantasia { get; set; }
    public string DFprovider { get; set; }
    public string DFdriver { get; set; }
    public string DFservidor { get; set; }
    public int DFporta { get; set; }
    public string DFdatabase { get; set; }
    public string DFusuario { get; set; }
    public string DFsenha { get; set; }
    public DateTime? DFdata_inativacao { get; set; }
    public DateTime? DFip_bloqueado_ate { get; set; }

    /// <summary>
    /// Obtém informações sobre as empresas cadastradas.
    /// </summary>
    /// <param name="cnDirector">Uma conexão ativa com o DBdirector.</param>
    /// <param name="stopToken">Um TOKEN para cancelamento assíncrono da tareafa.</param>
    /// <returns></returns>
    public static async Task<TBempresa_mercadologic[]> ObterAsync(
      DbConnection cnDirector, CancellationToken stopToken)
    {
      var empresas = await
        @"select TBempresa_mercadologic.DFcod_empresa
               , TBempresa.DFnome_fantasia
               , TBempresa_mercadologic.DFprovider
               , TBempresa_mercadologic.DFdriver
               , TBempresa_mercadologic.DFservidor
               , TBempresa_mercadologic.DFporta
               , TBempresa_mercadologic.DFdatabase 
               , TBempresa_mercadologic.DFusuario
               , TBempresa_mercadologic.DFsenha
	             , TBempresa.DFdata_inativacao
	             , (select DFbloqueado_ate
	                 from mlogic.TBacesso_mercadologic
                   where DFip = TBempresa_mercadologic.DFservidor
                 ) as DFip_bloqueado_ate
            from TBempresa_mercadologic with (nolock)
           inner join TBempresa with (nolock)
                   on TBempresa.DFcod_empresa = TBempresa_mercadologic.DFcod_empresa"
          .AsSql()
          .SelectAsync<TBempresa_mercadologic>(cnDirector, stopToken: stopToken);
      return empresas;
    }
  }
}