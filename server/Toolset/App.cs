using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace Toolset
{
  public static class App
  {
    static App()
    {
      App.Guid = FindGuid();
      App.Name = FindName();
      App.Path = FindPath();
      App.Version = FindVersion();
    }

    /// <summary>
    /// Identificador único da instância do aplicativo instalada.
    /// Por padrão o identificador é armazenado na pasta de instalação do
    /// aplicativo sob o nome de Guid.txt.
    /// </summary>
    public static Guid Guid { get; set; }

    /// <summary>
    /// Nome interno do aplicativo.
    /// </summary>
    public static string Name { get; set; }

    /// <summary>
    /// Caminho dos arquivos do aplicativo. Em geral corresponde à
    /// pasta de instalação.
    /// </summary>
    public static string Path { get; set; }

    public static string CommonPath { get; set; }

    /// <summary>
    /// Versão corrente do aplicativo.
    /// </summary>
    public static VersionInfo Version { get; set; }

    /// <summary>
    /// Descrição amigável opcional do aplicativo.
    /// </summary>
    public static string Description { get; set; }

    static Guid FindGuid()
    {
      Guid? guid = null;

      var path = FindPath();
      var filepath = System.IO.Path.Combine(path, "Guid.txt");

      if (File.Exists(filepath))
      {
        try
        {
          var content = File.ReadAllText(filepath).Trim();
          guid = Guid.Parse(content);
        }
        catch { /* Nada a fazer */ }
      }

      if (guid == null)
      {
        guid = Guid.NewGuid();
        try
        {
          var content = guid.Value.ToString("D");
          File.WriteAllText(filepath, content);
        }
        catch { /* Nada a fazer */ }
      }

      return guid.Value;
    }

    /// <summary>
    /// Determina o nome ideal para identificar este aplicativo.
    /// </summary>
    static string FindName()
    {
      try
      {
        var assembly = Assembly.GetEntryAssembly();
        var name = assembly?.FullName.Split(',').FirstOrDefault();
        if (name != null)
        {
          return name;
        }
      }
      catch { /* Nada a fazer. */ }

      try
      {
        var process = Process.GetCurrentProcess();
        var name = process.ProcessName.ChangeCase(TextCase.PascalCase);
        return name;
      }
      catch { /* Nada a fazer. */ }

      return "Aplicativo";
    }

    /// <summary>
    /// Obtém o caminho no qual o aplicativo está instalado.
    /// </summary>
    /// <returns>O caminho no qual o aplicativo está instalado.</returns>
    static string FindPath()
    {
      try
      {
        var local = Assembly.GetExecutingAssembly().Location;
        return System.IO.Path.GetDirectoryName(local);
      }
      catch
      {
        try
        {
          var local = Assembly.GetEntryAssembly().Location;
          return System.IO.Path.GetDirectoryName(local);
        }
        catch
        {
          var local = Directory.GetCurrentDirectory();
          return System.IO.Path.GetDirectoryName(local);
        }
      }
    }

    /// <summary>
    /// Obtém informação de revisão a partir de um arquivo chamado
    /// REVISION.txt na pasta onde o sistema está instalado.
    /// 
    /// É esperado que o arquivo tenha a versão do aplicativo na forma:
    /// -   X.X.X-sufixoX_rX
    /// Sendo:
    /// -   X
    ///         Um número qualquer.
    /// -   X.X.X
    ///         Obrigatório.
    ///         Número de versão do aplicativo.
    /// -   sufixoX
    ///         Opcional.
    ///         O nome da versão, como alfa, beta, trunk, etc.
    ///         Seguido opcionalmente de um número de revisão do sufixo.
    /// -   rX
    ///         Opcional.
    ///         O número de revisão no repositório de código fonte.
    /// </summary>
    /// <returns>
    /// A informação de versão obtida; Caso não exista, SNAPSHOT é retornado.
    /// </returns>
    static VersionInfo FindVersion()
    {
      string revisionFile = null;
      try
      {
        revisionFile = System.IO.Path.Combine(App.Path, "REVISION.txt");

        if (!File.Exists(revisionFile))
        {
          Trace.TraceWarning("Era esperado existir um arquivo chamado REVISION.txt na pasta onde o aplicativo foi instalado com o número de versão na forma 'X.X.X-sufixo_rX', mas este arquivo não foi encontrado.");
          return new VersionInfo();
        }

        var content = File.ReadAllText(revisionFile);
        var version = VersionInfo.Parse(content);

        return version;
      }
      catch (Exception ex)
      {
        ex.TraceWarning("Não possível extrair informação de versão do arquivo: " + revisionFile);
        return new VersionInfo();
      }
    }
  }
}
