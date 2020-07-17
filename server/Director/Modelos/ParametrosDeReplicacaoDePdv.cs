using System;
namespace Director.Modelos
{
  public class ParametrosDeReplicacaoDePdv
  {
    private THistorico _historico;

    public bool Ativado { get; set; }

    public THistorico Historico
    {
      get => _historico ?? (_historico = new THistorico());
      set => _historico = value;
    }

    public class THistorico
    {
      public bool Ativado { get; set; }
      public int Dias { get; set; }
    }
  }
}
