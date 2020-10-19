using System;
using System.IO;

namespace Mercadologic.Carga.Tipos
{
  public class PacoteDeCarga
  {
    public int Empresa { get; set; }

    public Versao Versao { get; set; }

    public string Rotulo { get; set; }

    public FileInfo ArquivoZip { get; set; }
  }
}
