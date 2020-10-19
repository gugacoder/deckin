﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mercadologic.Carga.Tipos
{
  /// <summary>
  /// Número de versão da carga conforme gerada pelo Concentrador.
  /// </summary>
  public class Versao
  {
    public int Numero { get; set; }
    public string Descricao { get; set; }
    public DateTime Data { get; set; } = DateTime.Now;

    public override string ToString() => $"at.{Numero} {Descricao}";
  }
}
