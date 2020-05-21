using System;
namespace Director.Domain.Aut
{
  public class Identidade
  {
    public int IdUsuario { get; set; }

    public string Usuario { get; set; }

    public int IdNivel { get; set; }

    public string Nivel { get; set; }

    public bool Autenticado { get; set; }
  }
}
