using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Keep.Tools;

namespace Keep.Hosting.Api
{
  public class WindowsServiceInstaller
  {
    private readonly WindowsServiceProperties properties;

    public WindowsServiceInstaller(WindowsServiceProperties options = null)
    {
      this.properties = options;
    }

    public void RunInstaller(string[] args)
    {
      var command = args.FirstOrDefault();
      switch (command)
      {
        case "/Install":
          OnInstall();
          break;

        case "/Commit":
          OnCommit();
          break;

        case "/Rollback":
          OnRollback();
          break;

        case "/Uninstall":
          OnUninstall();
          break;
      }
    }

    private void OnInstall()
    {
      var exe = properties?.Exe ?? WindowsServiceProperties.FindExe();
      var name = properties?.Name ?? App.Name;
      var title = properties?.Title ?? App.Title;
      var description = properties?.Description ?? App.Description;
      var manufacturer = properties?.Manufacturer ?? App.Manufacturer;

      var displayName = string.IsNullOrEmpty(manufacturer)
        ? title : $"{manufacturer} - {title}";
      
      RunCmd($"sc create {name} binPath= \"{exe}\" DisplayName= \"{displayName}\"");
      RunCmd($"sc description {name} \"{description}\"");
      RunCmd($"sc failure {name} actions= restart/60000/restart/60000/\"\"/60000 reset= 86400");
      RunCmd($"sc config {name} start= auto");
      RunCmd($"sc start {name}");
    }

    private void OnCommit()
    {
    }

    private void OnRollback()
    {
      OnUninstall();
    }

    private void OnUninstall()
    {
      try
      {
        var name = properties?.Name ?? App.Name;

        RunCmd($"sc stop {name}", ignoreFailures: true);

        var trials = 10; // 10segundos
        while (trials-- > 0)
        {
          Thread.Sleep(1000);

          var result = RunCmd($"sc query {name}", ignoreFailures: true);
          var stopped = result.ContainsIgnoreCase("STOPPED");
          if (stopped)
            break;
        }

        RunCmd($"sc delete {name}", ignoreFailures: true);
      }
      catch (Exception ex)
      {
        try
        {
          DumpToLog(ex);
        }
        catch { /* Nada mais a fazer. */ }
      }
    }

    /// <summary>
    /// Executa um comando do shell.
    /// </summary>
    /// <param name="command">O comando a ser executado.</param>
    /// <param name="ignoreFailures">Ignora falhas.</param>
    /// <returns>A saída produzida pelo comando.</returns>
    private static string RunCmd(string command, bool ignoreFailures = false)
    {
      var ok = RunCmd(command,
        emitStdOutput: true,
        emitErrOutput: true,
        stdOutput: out string stdOutput,
        errOutput: out string errOutput
      );

      DumpToLog(stdOutput, errOutput);

      if (!ok && !ignoreFailures)
        throw new Exception("Falhou a tentativa de instalar o serviço do Windows.");

      return stdOutput;
    }

    private static void DumpToLog(params object[] content)
    {
      var logFile = $"{App.Path}\\WindowsServiceInstaller.log";

      EnsureDirectory(logFile);
      File.AppendAllText(logFile, Environment.NewLine);
      File.AppendAllText(logFile, DateTime.Now.ToString());
      File.AppendAllText(logFile, Environment.NewLine);

      foreach (var item in content)
      {
        var text = (item is Exception ex) ? ex.GetStackTrace() : item?.ToString();
        File.AppendAllText(logFile, text);
      }
    }

    #region Shell

    /// <summary>
    /// Executa um comando do shell.
    /// </summary>
    /// <param name="command">O comando a ser executado.</param>
    /// <param name="emitStdOutput">Diz se a saída padrão deve ser resultada.</param>
    /// <param name="emitErrOutput">Diz se a saída de erro deve ser resultada.</param>
    /// <param name="stdOutput">A saída padrão.</param>
    /// <param name="errOutput">A saída de erro.</param>
    /// <returns>Verdadeiro se o comando foi bem sucedido.</returns>
    private static bool RunCmd(string command, bool emitStdOutput,
      bool emitErrOutput, out string stdOutput, out string errOutput)
    {
      string stdOutputFile = null;
      string errOutputFile = null;

      stdOutput = null;
      errOutput = null;

      try
      {
        var regex = new Regex(" (2?>)([^>]*)");
        var matches = regex.Matches(command);
        foreach (var match in matches.Cast<Match>())
        {
          var kind = match.Groups[1].Value;
          var path = match.Groups[2].Value.Trim().Replace("'", "\"");

          if (kind == "2>")
            errOutputFile = path;
          else
            stdOutputFile = path;

          var text = match.Captures[0].Value;
          command = command.Replace(text, "");
        }

        var info = ParseCommand(command);
        info.UseShellExecute = false;
        info.RedirectStandardOutput = emitStdOutput || (stdOutputFile != null);
        info.RedirectStandardError = emitErrOutput || (errOutputFile != null);

        EnsureDirectory(stdOutputFile);
        EnsureDirectory(errOutputFile);

        var redirects = "";
        if (emitStdOutput) redirects += $" > nul";
        if (emitErrOutput) redirects += $" 2> nul";
        if (stdOutputFile != null) redirects += $" > \"{stdOutputFile}\"";
        if (errOutputFile != null) redirects += $" 2> \"{errOutputFile}\"";

        using (var process = Process.Start(info))
        {
          if (emitStdOutput)
          {
            stdOutput = process.StandardOutput.ReadToEnd().Trim();
            process.StandardOutput.Close();
          }
          else if (stdOutputFile != null)
          {
            File.WriteAllText(stdOutputFile, process.StandardOutput.ReadToEnd());
            process.StandardOutput.Close();
          }

          if (emitErrOutput)
          {
            errOutput = process.StandardError.ReadToEnd().Trim();
            process.StandardError.Close();
          }
          else if (errOutputFile != null)
          {
            File.WriteAllText(errOutputFile, process.StandardError.ReadToEnd());
            process.StandardError.Close();
          }

          process.WaitForExit();

          return process.ExitCode == 0;
        }
      }
      catch (Exception ex)
      {
        if (emitErrOutput)
        {
          errOutput = ex.GetStackTrace();
        }
        else if (errOutputFile != null)
        {
          EnsureDirectory(errOutputFile);
          File.AppendAllText(errOutputFile, ex.GetStackTrace());
        }
        return false;
      }
    }

    private static ProcessStartInfo ParseCommand(string command)
    {
      string cmd;
      string arg;

      command = command.Replace("'", "\"");

      if (command.StartsWith("\""))
      {
        var index = command.IndexOf('"', 1);
        cmd = command.Substring(1, index - 1);
        arg = command.Substring(index + 1).Trim();
      }
      else
      {
        cmd = command.Split(' ').First();
        arg = command.Substring(cmd.Length).Trim();
      }

      var info = new ProcessStartInfo(cmd, arg);
      return info;
    }

    private static void EnsureDirectory(string file)
    {
      if (string.IsNullOrEmpty(file))
        return;

      var folder = Path.GetDirectoryName(file);
      if (string.IsNullOrEmpty(folder))
        return;

      Directory.CreateDirectory(folder);
    }

    #endregion
  }
}
