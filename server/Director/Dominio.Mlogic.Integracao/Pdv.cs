using System;
namespace Director.Dominio.Mlogic.Integracao
{
  public class Pdv
  {
    public int IdPdv { get; set; }
    public int IdEmpresa { get; set; }
    public string Descricao { get; set; }
    public string EnderecoDeRede { get; set; }
    public bool Ativado { get; set; }
    public bool ReplicacaoAtivado { get; set; }
  }
}
