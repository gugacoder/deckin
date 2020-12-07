using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Keep.Paper.Api;

namespace Keep.Paper.Hosting
{
  public class Settings : ISettings
  {
    private readonly string folder;

    public Settings(string folder, ISettings fallback = null)
    {
      this.folder = folder;
      this.FallbackSettings = fallback;
    }

    public ISettings FallbackSettings { get; }

    public async Task<string> GetAsync(string key,
      CancellationToken stopToken = default)
    {
      string value = null;

      var file = Path.GetFullPath(Path.Combine(folder, $"{key}.key"));
      if (File.Exists(file))
      {
        var text = await File.ReadAllTextAsync(file, stopToken);
        value = Crypto.Decrypt(text);
      }

      if (value == null && FallbackSettings != null)
      {
        value = await FallbackSettings.GetAsync(key, stopToken);
      }

      return value;
    }

    public async Task SetAsync(string key, string value,
      CancellationToken stopToken = default)
    {
      var file = Path.GetFullPath(Path.Combine(folder, $"{key}.key"));
      var path = Path.GetDirectoryName(file);
      if (!Directory.Exists(path))
      {
        Directory.CreateDirectory(path);
      }

      var text = Crypto.Encrypt(value);
      await File.WriteAllTextAsync(file, text, stopToken);
    }
  }
}
