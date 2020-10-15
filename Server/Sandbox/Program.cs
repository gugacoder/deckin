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

namespace Director
{
  public class Program
  {
    [SuppressMessage("Security", "CA2100:Review SQL queries for security vulnerabilities", Justification = "<Pending>")]
    public static void Main(string[] args)
    {
      // Sandbox.ips Sandbox.sql
      try
      {
        var ipFile = args.FirstOrDefault();
        var sqlFile = args.Skip(1).FirstOrDefault();

        if (string.IsNullOrEmpty(ipFile) || string.IsNullOrEmpty(sqlFile))
        {
          Console.WriteLine($"Uso incorreto.");
          Console.WriteLine($"  Informe o arquivo de IPs de PDV e o arquivo contendo a SQL.");
          Console.WriteLine($"  Exemplo:");
          Console.WriteLine($"    {App.Name} IPs.txt Comando.sql");
          return;
        }

        ipFile = Path.GetFullPath(ipFile);
        sqlFile = Path.GetFullPath(sqlFile);

        Console.WriteLine();
        Console.WriteLine($"IP-FILE: {ipFile}");
        Console.WriteLine($"SQL-FILE: {sqlFile}");
        Console.WriteLine();

        var ips = File
          .ReadAllLines(ipFile)
          .Select(ip => ip.Split('#').First())
          .Select(ip => ip.Trim())
          .NotNullOrWhitespace()
          .ToArray();
        var sql = File.ReadAllText(sqlFile);

        Console.WriteLine($"IPs afetados:");
        foreach (var ip in ips)
        {
          Console.WriteLine($"- {ip}");
        }

        foreach (var ip in ips)
        {
          try
          {
            Console.WriteLine();
            Console.WriteLine("- - -");
            Console.WriteLine($"IP: {ip}");
            Console.WriteLine("Conectando...");

            using var cn = new Npgsql.NpgsqlConnection();
            cn.ConnectionString = $"server={ip};database=DBPDV;uid=postgres;pwd=local";
            cn.Open();

            Console.WriteLine("Executando...");

            using var cm = cn.CreateCommand();
            cm.CommandText = sql;
            var reader = cm.ExecuteReader();

            PrintReader(reader);

            Console.WriteLine();
            Console.WriteLine("DONE!");
          }
          catch (Exception ex)
          {
            Console.WriteLine("FAIL!");
            Console.WriteLine(ex.GetCauseMessage());
            Console.WriteLine(ex.GetStackTrace());
          }
          finally
          {
            Console.WriteLine("- - -");
          }
        }

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

    private static void PrintReader(Npgsql.NpgsqlDataReader reader)
    {
      Console.WriteLine();
      do
      {
        while (reader.Read())
        {
          for (var i = 0; i < reader.FieldCount; i++)
          {
            var field = reader.GetName(i);
            var value = reader.GetValue(i);

            Console.WriteLine($"{field}: {value}");
          }
        }
      } while (reader.NextResult());
      Console.WriteLine();
    }
  }
}
