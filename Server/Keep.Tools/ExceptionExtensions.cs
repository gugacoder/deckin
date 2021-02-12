using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace Keep.Tools
{
  public static class ExceptionExtensions
  {
    public static string GetStackTrace(this Exception exception)
    {
      using (var saida = new StringWriter())
      {
        Trace(exception, saida);
        return saida.ToString();
      }
    }

    public static Exception Debug(this Exception exception, string message = null)
    {
      try
      {
        var pilha = GetStackTrace(exception);
        if (!string.IsNullOrEmpty(message))
        {
          pilha = $"{message}\n{pilha}";
        }
        System.Diagnostics.Debug.WriteLine($"[EXCEPTION] {pilha}");
      }
      catch (Exception ex)
      {
        Dump(exception, ex);
      }
      return exception;
    }

    public static Exception Trace(this Exception exception)
    {
      try
      {
        var pilha = GetStackTrace(exception);
        System.Diagnostics.Trace.TraceError(pilha);
      }
      catch (Exception ex)
      {
        Dump(exception, ex);
      }
      return exception;
    }

    public static Exception TraceWarning(this Exception exception)
    {
      var pilha = GetStackTrace(exception);
      try
      {
        System.Diagnostics.Trace.TraceWarning(pilha);
      }
      catch (Exception ex)
      {
        Dump(exception, ex);
      }
      return exception;
    }

    public static Exception Trace(this Exception exception, string mensagem)
    {
      var pilha = GetStackTrace(exception);
      try
      {
        System.Diagnostics.Trace.TraceError("{0}\nCausa:\n{1}", mensagem, pilha);
      }
      catch (Exception ex)
      {
        Dump(exception, ex);
      }
      return exception;
    }

    public static Exception Trace(this Exception exception, string formato, params object[] args)
    {
      Trace(exception, string.Format(formato, args));
      return exception;
    }

    public static Exception TraceWarning(this Exception exception, string mensagem)
    {
      try
      {
        var pilha = GetStackTrace(exception);
        System.Diagnostics.Trace.TraceWarning("{0}\nCausa:\n{1}", mensagem, pilha);
      }
      catch (Exception ex)
      {
        Dump(exception, ex);
      }
      return exception;
    }

    public static Exception TraceWarning(this Exception exception, string formato, params object[] args)
    {
      TraceWarning(exception, string.Format(formato, args));
      return exception;
    }

    public static Exception Trace(this Exception exception, Stream saida)
    {
      using (var gravador = new StreamWriter(saida))
      {
        Trace(exception, gravador);
      }
      return exception;
    }

    public static Exception Trace(this Exception exception, TextWriter saida)
    {
      saida.Write("fault ");
      Exception ex = exception;
      do
      {
        saida.WriteLine(ex.Message);
        saida.Write(" type ");
        saida.WriteLine(ex.GetType().FullName);

        var trace = ex.StackTrace;
        if (trace != null)
        {
          saida.Write(trace);
          if (!trace.EndsWith(Environment.NewLine))
          {
            saida.WriteLine();
          }
        }

        ex = ex.InnerException;
        if (ex != null)
        {
          saida.Write("cause ");
        }
      } while (ex != null);
      return exception;
    }

    public static Exception[] GetCauses(this Exception exception)
    {
      var causes = EnumerateExceptionCauses(exception).ToArray();
      return causes;
    }

    public static string[] GetCauseMessages(this Exception exception)
    {
      var causes =
        EnumerateExceptionCauses(exception)
          .Select(x => x.Message ?? "Falha não identificada.")
          .Distinct()
          .ToArray();
      return causes;
    }

    public static string GetCauseMessage(this Exception exception)
    {
      return string.Join(Environment.NewLine, exception.GetCauseMessages());
    }

    private static IEnumerable<Exception> EnumerateExceptionCauses(Exception exception)
    {
      while (exception != null)
      {
        yield return exception;
        exception = exception.InnerException;
      }
    }

    /// <summary>
    /// Este método tenta registrar as exceções da forma possível.
    /// Deve ser usado em caso de falha do método geral de gravação de
    /// exceções.
    /// </summary>
    /// <param name="excecoes">As exceções a serem gravadas.</param>
    private static void Dump(params Exception[] excecoes)
    {
      foreach (var exception in excecoes)
      {
        try
        {
          var pilha = GetStackTrace(exception);

          Console.Write("[FALHA]");
          Console.WriteLine(exception.Message);
          Console.WriteLine(pilha);

          System.Diagnostics.Debug.Write("[FALHA]");
          System.Diagnostics.Debug.WriteLine(exception.Message);
          System.Diagnostics.Debug.WriteLine(pilha);
        }
        catch
        {
          // nada a fazer
        }
      }
    }

  }
}
