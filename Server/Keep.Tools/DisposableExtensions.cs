using System;
using System.Collections.Generic;
using System.Text;

namespace Keep.Tools
{
  public static class DisposableExtensions
  {
    /// <summary>
    /// Executa o método Dispose do objeto e ignora erros ocorridos.
    /// </summary>
    /// <param name="disposable"></param>
    public static Ret TryDispose(this IDisposable disposable)
    {
      try
      {
        disposable?.Dispose();
        return true;
      }
      catch (Exception ex)
      {
        return ex;
      }
    }
  }
}
