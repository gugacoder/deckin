using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Keep.Tools;
using Keep.Tools.Collections;
using Keep.Tools.IO;

namespace Keep.Hosting.Client
{
  public class FileBrowser
  {
    private string separator;
    private string invalidSeparator;

    public FileBrowser()
    {
      this.separator = DetectSeparator();
      this.invalidSeparator = (this.separator == "/") ? "\\" : "/";
    }

    private string DetectSeparator()
    {
      var assembly = GetType().Assembly;
      var manifests = assembly.GetManifestResourceNames();
      var useSlash = manifests.Any(x => x.Contains("/"));
      return useSlash ? "/" : "\\";
    }

    public static string GetName(string filepath)
    {
      var path = GetPath(filepath);
      return path.Split('/').NotNullOrEmpty().LastOrDefault();
    }

    public static string GetPath(string filepath)
    {
      return $"/{filepath.Replace("\\", "/")}";
    }

    private string GetResourcePath(string filepath)
    {
      filepath = filepath.Replace(invalidSeparator, separator);
      if (filepath.StartsWith(separator)) filepath = filepath.Substring(1);
      return filepath;
    }

    private string GetResourceFolder(string filepath)
    {
      filepath = filepath.Replace(invalidSeparator, separator);
      if (!filepath.EndsWith(separator)) filepath += separator;
      if (filepath.StartsWith(separator)) filepath = filepath.Substring(1);
      return filepath;
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

      var pattern = (separator == "/")
        ? $"^({folder}[^\\\\]+\\\\)"
        : $"^({folder}[^/]+/)";

      var regex = new Regex(pattern);

      var assembly = GetType().Assembly;
      var files = assembly.GetManifestResourceNames();

      foreach (var file in files)
      {
        var match = regex.Match(file);
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

      var pattern = (separator == "/")
        ? $"^{folder}[^\\\\]+$"
        : $"^{folder}[^/]+$";

      var regex = new Regex(pattern);

      var assembly = GetType().Assembly;
      var files = assembly.GetManifestResourceNames();

      foreach (var file in files)
      {
        if (regex.IsMatch(file)) yield return GetPath(file);
      }
    }

    public IEnumerable<string> EnumerateFilesRecursively(string folder)
    {
      if (!FolderExists(folder))
        throw new FileNotFoundException(
          $"A pasta não existe: {GetPath(folder)}");

      folder = GetResourceFolder(folder);
      folder = folder.Replace("\\", "\\\\");

      var regex = new Regex($"^{folder}.*$");

      var assembly = GetType().Assembly;
      var files = assembly.GetManifestResourceNames();

      foreach (var file in files)
      {
        if (regex.IsMatch(file)) yield return GetPath(file);
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

        var regex = new Regex($"^{folder}.+");

        var assembly = GetType().Assembly;
        var files = assembly.GetManifestResourceNames();

        return files.Any(regex.IsMatch);
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

        filepath = GetResourcePath(filepath);

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
      filepath = GetResourcePath(filepath);

      var assembly = GetType().Assembly;

      var stream = assembly.GetManifestResourceStream(filepath);
      if (stream == null)
        throw new FileNotFoundException(
          $"O arquivo não existe: {GetPath(filepath)}");

      return stream;
    }

    public TextReader OpenText(string filepath)
    {
      filepath = GetResourcePath(filepath);

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
      filepath = GetResourcePath(filepath);

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
      filepath = GetResourcePath(filepath);

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
      filepath = GetResourcePath(filepath);

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
      filepath = GetResourcePath(filepath);

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
      filepath = GetResourcePath(filepath);

      var assembly = GetType().Assembly;

      using var stream = assembly.GetManifestResourceStream(filepath);
      if (stream == null)
        throw new FileNotFoundException(
          $"O arquivo não existe: {GetPath(filepath)}");

      stream.CopyTo(output);
    }

    public async Task CopyFileToAsync(string filepath, Stream output)
    {
      filepath = GetResourcePath(filepath);

      var assembly = GetType().Assembly;

      using var stream = assembly.GetManifestResourceStream(filepath);
      if (stream == null)
        throw new FileNotFoundException(
          $"O arquivo não existe: {GetPath(filepath)}");

      await stream.CopyToAsync(output);
    }

    public void CopyTextTo(string filepath, TextWriter writer)
    {
      filepath = GetResourcePath(filepath);

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
      filepath = GetResourcePath(filepath);

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
