using System;
using System.Threading;
using System.Threading.Tasks;
using Keep.Tools;
using Keep.Tools.Reflection;

namespace Keep.Hosting
{
  public static class SettingsExtensions
  {
    #region Async Extenstions

    public static async Task<T> GetAsync<T>(this ISettings settings, string key,
      CancellationToken stopToken = default)
    {
      var value = await settings.GetAsync(key, stopToken);
      var castValue = Change.To<T>(value);
      return castValue;
    }

    public static async Task SetAsync(this ISettings settings, string key,
      object value, CancellationToken stopToken = default)
    {
      var text = Change.To<string>(value);
      await settings.SetAsync(key, text, stopToken);
    }

    public static async Task<T> FillAsync<T>(this ISettings settings,
      string section, CancellationToken stopToken = default)
      where T : new()
    {
      var @object = new T();
      await CopySettingsAsync(settings, section, @object, stopToken);
      return @object;
    }

    public static async Task<object> FillAsync(this ISettings settings, string section,
      Type type, CancellationToken stopToken = default)
    {
      var @object = Activator.CreateInstance(type);
      await CopySettingsAsync(settings, section, @object, stopToken);
      return @object;
    }

    public static async Task FillAsync(this ISettings settings, string section,
      object target, CancellationToken stopToken = default)
    {
      await CopySettingsAsync(settings, section, target, stopToken);
    }

    private static async Task CopySettingsAsync(ISettings settings,
      string section, object target, CancellationToken stopToken = default)
    {
      var names = target._Keys();
      foreach (var name in names)
      {
        var key = $"{section}.{name}";
        var value = await settings.GetAsync(key, stopToken);
        target._Set(name, value);
      }
    }

    #endregion

    #region Sync Extenstions

    public static string Get(this ISettings settings, string key)
      => settings.GetAsync(key).Await();

    public static T Get<T>(this ISettings settings, string key)
      where T : new()
      => settings.GetAsync<T>(key).Await();

    public static void Set(this ISettings settings, string key, string value)
      => settings.SetAsync(key, value).Await();

    public static void Set(this ISettings settings, string key, object value)
      => settings.SetAsync(key, value).Await();

    public static T Fill<T>(this ISettings settings, string section)
      where T : new()
      => settings.FillAsync<T>(section).Await();

    public static object Fill(this ISettings settings, string section, Type type)
      => settings.FillAsync(section, type).Await();

    public static void Fill(this ISettings settings, string section, object target)
      => settings.FillAsync(section, target).Await();

    #endregion

  }
}
