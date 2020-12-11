using System;
using System.Diagnostics;
using Keep.Tools;
using Keep.Tools.Collections;
using System.Linq;

namespace Mercadologic.Replicacao
{
  public class Program
  {
    public static void Main(string[] args)
    {
      try
      {

      }
      catch (Exception ex)
      {
        Console.WriteLine("FAIL!!!");
        Console.WriteLine(ex.GetCauseMessage());
        Console.WriteLine(ex.GetStackTrace());
      }
    }
  }
}
