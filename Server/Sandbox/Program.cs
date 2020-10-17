using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using Keep.Paper.Api;
using Keep.Paper.Api.Types;
using Keep.Paper.Helpers;
using Keep.Tools;
using Keep.Tools.Sequel;
using Keep.Tools.Sequel.Runner;
using Keep.Tools.Xml;
using Keep.Tools.Collections;
using System.Xml.Linq;
using System.Linq;
using Keep.Tools.Reflection;
using System.Collections;
using System.Xml.Serialization;
using Keep.Paper.Templating;
using System.Diagnostics.CodeAnalysis;
using Mercadologic.Carga.Utilitarios;
using System.Data;

namespace Mercadologic.Replicacao
{
  public class Program
  {
    [SuppressMessage("Security", "CA2100:Review SQL queries for security vulnerabilities", Justification = "<Pending>")]
    public static void Main(string[] args)
    {
      try
      {
        var file = "/tmp/file.json";

        SalvarJson(file);

        Console.WriteLine(Files.LoadTextFile(file));

        Console.WriteLine();
        Console.WriteLine("DONE!!!");
      }
      catch (Exception ex)
      {
        Console.WriteLine("FAIL!!!");
        Console.WriteLine(ex.GetCauseMessage());
        Console.WriteLine(ex.GetStackTrace());
      }
    }

    private static void SalvarJson(string arquivo)
    {
      using var stream = new StreamWriter(arquivo);
      using var gravador = new GravadorDeJson(stream);

      gravador.BeginArray();

      gravador.BeginObject();
      gravador.WriteProperty("id", -1);
      gravador.WriteProperty("name", "Yesterday");
      gravador.WriteProperty("date", DateTime.Today.AddDays(-1));
      gravador.EndObject();

      gravador.BeginObject();
      gravador.WriteProperty("id", 0);
      gravador.WriteProperty("name", "Today");
      gravador.WriteProperty("date", DateTime.Today);
      gravador.EndObject();

      gravador.BeginObject();
      gravador.WriteProperty("id", 1);
      gravador.WriteProperty("name", "Tomorrow");
      gravador.WriteProperty("date", DateTime.Today.AddDays(1));
      gravador.EndObject();

      gravador.EndArray();
    }
  }
}
