using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Keep.Tools.IO
{
  public static class Extensions
  {
    /// <summary>
    /// Copia o texto do fluxo de leitura para o fluxo de escrita.
    /// O método Flush do fluxo de escrita é executado automaticamente ao fim da cópia.
    /// </summary>
    /// <param name="reader">O fluxo de leitura.</param>
    /// <param name="writer">O fluxo de saída.</param>
    public static void CopyTo(this TextReader reader, TextWriter writer)
    {
      var buffer = new char[8 * 1024];
      var len = 0;
      while ((len = reader.Read(buffer, 0, buffer.Length)) > 0)
      {
        writer.Write(buffer, 0, len);
      }
      writer.Flush();
    }

    /// <summary>
    /// Copia o texto do fluxo de leitura para o fluxo de escrita assincronamente.
    /// O método Flush do fluxo de escrita é executado automaticamente ao fim da cópia.
    /// </summary>
    /// <param name="reader">O fluxo de leitura.</param>
    /// <param name="writer">O fluxo de saída.</param>
    public static async Task CopyToAsync(this TextReader reader, TextWriter writer)
    {
      var buffer = new char[8 * 1024];
      var len = 0;
      while ((len = await reader.ReadAsync(buffer, 0, buffer.Length)) > 0)
      {
        await writer.WriteAsync(buffer, 0, len);
      }
      await writer.FlushAsync();
    }
  }
}
