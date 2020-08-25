using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Keep.Tools;
using Keep.Tools.Collections;

namespace Keep.Paper.Client
{
  public class FileBrowser
  {
    public static string GetName(string filepath)
    {
      var path = GetPath(filepath);
      return path.Split('/').NotNullOrEmpty().LastOrDefault();
    }

    public static string GetPath(string filepath)
    {
      return $"/{filepath.Replace("\\", "/")}";
    }

    private string GetResourceFolder(string folder)
    {
      folder = folder.Replace("/", "\\");
      if (!folder.EndsWith("\\")) folder += "\\";
      if (folder.StartsWith("\\")) folder = folder.Substring(1);
      return folder;
    }

    public IEnumerable<string> EnumerateFolders(string folder)
    {
      if (!FolderExists(folder))
        throw new FileNotFoundException(
          $"A pasta não existe: {GetPath(folder)}");

      return MatchFolders(folder).Distinct();
    }

    private IEnumerable<string> MatchFolders(string folder)
    {
      folder = GetResourceFolder(folder);
      folder = folder.Replace("\\", "\\\\");

      var pattern = new Regex($"^({folder}[^\\\\]+\\\\)");

      var assembly = GetType().Assembly;
      var files = assembly.GetManifestResourceNames();

      foreach (var file in files)
      {
        var match = pattern.Match(file);
        if (match.Success) yield return GetPath(match.Groups[1].Value);
      }
    }
    
    public IEnumerable<string> EnumerateFiles(string folder)
    {
      if (!FolderExists(folder))
        throw new FileNotFoundException(
          $"A pasta não existe: {GetPath(folder)}");

      folder = GetResourceFolder(folder);
      folder = folder.Replace("\\", "\\\\");

      var pattern = new Regex($"^{folder}[^\\\\]+$");

      var assembly = GetType().Assembly;
      var files = assembly.GetManifestResourceNames();

      foreach (var file in files)
      {
        if (pattern.IsMatch(file)) yield return GetPath(file);
      }
    }

    public IEnumerable<string> EnumerateFilesRecursively(string folder)
    {
      if (!FolderExists(folder))
        throw new FileNotFoundException(
          $"A pasta não existe: {GetPath(folder)}");

      folder = GetResourceFolder(folder);
      folder = folder.Replace("\\", "\\\\");

      var pattern = new Regex($"^{folder}.*$");

      var assembly = GetType().Assembly;
      var files = assembly.GetManifestResourceNames();
      
      foreach (var file in files)
      {
        if (pattern.IsMatch(file)) yield return GetPath(file);
      }
    }

    public bool FolderExists(string folder)
    {
      try
      {
        if (string.IsNullOrEmpty(folder))
          return false;

        folder = GetResourceFolder(folder);

        folder = folder.Replace("\\", "\\\\");
        var pattern = new Regex($"^{folder}.+");

        var assembly = GetType().Assembly;
        var files = assembly.GetManifestResourceNames();

        return files.Any(pattern.IsMatch);
      }
      catch
      {
        return false;
      }
    }

    public bool FileExists(string filepath)
    {
      try
      {
        if (string.IsNullOrEmpty(filepath))
          return false;

        filepath = filepath.Replace("/", "\\");
        if (filepath.StartsWith("\\")) filepath = filepath.Substring(1);

        var assembly = GetType().Assembly;

        var info = assembly.GetManifestResourceInfo(filepath);
        return info != null;
      }
      catch
      {
        return false;
      }
    }

    public Stream OpenFile(string filepath)
    {
      filepath = filepath.Replace("/", "\\");
      if (filepath.StartsWith("\\")) filepath = filepath.Substring(1);

      var assembly = GetType().Assembly;

      var stream = assembly.GetManifestResourceStream(filepath);
      if (stream == null)
        throw new FileNotFoundException(
          $"O arquivo não existe: {GetPath(filepath)}");

      return stream;
    }

    public TextReader OpenText(string filepath)
    {
      filepath = filepath.Replace("/", "\\");
      if (filepath.StartsWith("\\")) filepath = filepath.Substring(1);

      var assembly = GetType().Assembly;

      var stream = assembly.GetManifestResourceStream(filepath);
      if (stream == null)
        throw new FileNotFoundException(
          $"O arquivo não existe: {GetPath(filepath)}");

      var reader = new StreamReader(stream,
        detectEncodingFromByteOrderMarks: true, leaveOpen: false);

      return reader;
    }

    public byte[] ReadFile(string filepath)
    {
      filepath = filepath.Replace("/", "\\");
      if (filepath.StartsWith("\\")) filepath = filepath.Substring(1);

      var assembly = GetType().Assembly;

      using var stream = assembly.GetManifestResourceStream(filepath);
      if (stream == null)
        throw new FileNotFoundException(
          $"O arquivo não existe: {GetPath(filepath)}");

      using var memory = new MemoryStream();

      stream.CopyTo(memory);
      var array = memory.ToArray();

      return array;
    }

    public async Task<byte[]> ReadFileAsync(string filepath)
    {
      filepath = filepath.Replace("/", "\\");
      if (filepath.StartsWith("\\")) filepath = filepath.Substring(1);

      var assembly = GetType().Assembly;

      using var stream = assembly.GetManifestResourceStream(filepath);
      if (stream == null)
        throw new FileNotFoundException(
          $"O arquivo não existe: {GetPath(filepath)}");

      using var memory = new MemoryStream();

      await stream.CopyToAsync(memory);
      var array = memory.ToArray();

      return array;
    }

    public string ReadText(string filepath)
    {
      filepath = filepath.Replace("/", "\\");
      if (filepath.StartsWith("\\")) filepath = filepath.Substring(1);

      var assembly = GetType().Assembly;

      using var stream = assembly.GetManifestResourceStream(filepath);
      if (stream == null)
        throw new FileNotFoundException(
          $"O arquivo não existe: {GetPath(filepath)}");

      using var reader = new StreamReader(stream);
      
      var text = reader.ReadToEnd();
      return text;
    }

    public async Task<string> ReadTextAsync(string filepath)
    {
      filepath = filepath.Replace("/", "\\");
      if (filepath.StartsWith("\\")) filepath = filepath.Substring(1);

      var assembly = GetType().Assembly;

      using var stream = assembly.GetManifestResourceStream(filepath);
      if (stream == null)
        throw new FileNotFoundException(
          $"O arquivo não existe: {GetPath(filepath)}");

      using var reader = new StreamReader(stream);

      var text = await reader.ReadToEndAsync();
      return text;
    }

    public void CopyFileTo(string filepath, Stream output)
    {
      filepath = filepath.Replace("/", "\\");
      if (filepath.StartsWith("\\")) filepath = filepath.Substring(1);

      var assembly = GetType().Assembly;

      using var stream = assembly.GetManifestResourceStream(filepath);
      if (stream == null)
        throw new FileNotFoundException(
          $"O arquivo não existe: {GetPath(filepath)}");

      stream.CopyTo(output);
    }

    public async Task CopyFileToAsync(string filepath, Stream output)
    {
      filepath = filepath.Replace("/", "\\");
      if (filepath.StartsWith("\\")) filepath = filepath.Substring(1);

      var assembly = GetType().Assembly;

      using var stream = assembly.GetManifestResourceStream(filepath);
      if (stream == null)
        throw new FileNotFoundException(
          $"O arquivo não existe: {GetPath(filepath)}");

      await stream.CopyToAsync(output);
    }

    public void CopyTextTo(string filepath, TextWriter writer)
    {
      filepath = filepath.Replace("/", "\\");
      if (filepath.StartsWith("\\")) filepath = filepath.Substring(1);

      var assembly = GetType().Assembly;

      using var stream = assembly.GetManifestResourceStream(filepath);
      if (stream == null)
        throw new FileNotFoundException(
          $"O arquivo não existe: {GetPath(filepath)}");

      using var reader = new StreamReader(stream);

      reader.CopyTo(writer);
    }
    
    public async Task CopyTextToAsync(string filepath, TextWriter writer)
    {
      filepath = filepath.Replace("/", "\\");
      if (filepath.StartsWith("\\")) filepath = filepath.Substring(1);

      var assembly = GetType().Assembly;

      using var stream = assembly.GetManifestResourceStream(filepath);
      if (stream == null)
        throw new FileNotFoundException(
          $"O arquivo não existe: {GetPath(filepath)}");

      using var reader = new StreamReader(stream);

      await reader.CopyToAsync(writer);
    }
  }
}
