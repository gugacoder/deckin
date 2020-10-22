using System;
using System.IO;

namespace Mercadologic.Tipos
{
  public class PacoteDeCarga
  {
    public int Empresa { get; set; }

    public VersaoDeCarga Versao { get; set; }

    public string Rotulo { get; set; }

    public FileInfo ArquivoZip { get; set; }
  }
}
