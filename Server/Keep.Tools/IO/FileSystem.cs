using System;
using System.IO;

namespace Keep.Tools.IO
{
  public static class FileSystem
  {
    public static string CreateTempFile(string suggestedName = null)
    {
      if (string.IsNullOrEmpty(suggestedName))
        return Path.GetTempFileName();

      var folder = CreateTempFolder();
      var filepath = Path.Combine(folder, suggestedName);
      return filepath;
    }

    public static string CreateTempFolder()
    {
      var path = Path.GetTempPath();
      File.Delete(path);
      Directory.CreateDirectory(path);
      return path;
    }

    public static string LoadTextFile(string filepath)
    {
      try
      {
        filepath = Path.GetFullPath(filepath);
        return File.ReadAllText(filepath);
      }
      catch (Exception ex)
      {
        throw new IOException($"{ex.Message} - {filepath}", ex);
      }
    }

    public static string LoadTextFileIfExists(string filepath)
    {
      try
      {
        filepath = Path.GetFullPath(filepath);
        if (File.Exists(filepath))
        {
          return File.ReadAllText(filepath);
        }
        return null;
      }
      catch (Exception ex)
      {
        throw new IOException($"{ex.Message} - {filepath}", ex);
      }
    }

    public static void SaveTextFile(string filepath, string content)
    {
      try
      {
        filepath = Path.GetFullPath(filepath);

        var folder = Path.GetDirectoryName(filepath);
        if (!Directory.Exists(folder))
        {
          Directory.CreateDirectory(folder);
        }

        File.WriteAllText(filepath, content);
      }
      catch (Exception ex)
      {
        throw new IOException($"{ex.Message} - {filepath}", ex);
      }
    }
  }
}
