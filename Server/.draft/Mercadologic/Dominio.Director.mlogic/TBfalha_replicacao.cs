using System;
namespace Mercadologic.Dominio.Director.mlogic
{
  public class TBfalha_replicacao
  {
    public const string EventoLocalizarPdvs = "localizar-pdvs";
    public const string EventoReplicar = "replicar";
    public const string EventoApagarHistorico = "apagar-historico";

    public int? DFid_falha_replicacao { get; set; }
    public string DFevento { get; set; }
    public DateTime? DFdata_falha { get; set; }
    public string DFfalha { get; set; }
    public string DFfalha_detalhada { get; set; }
    public int? DFcod_empresa { get; set; }
    public int? DFcod_pdv { get; set; }
    public string DFtabela { get; set; }
  }
}
