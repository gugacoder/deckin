using System;
using System.Threading;
using System.Threading.Tasks;

namespace Keep.Hosting
{
  public interface ISettings
  {
    Task AwaitAvailability();

    Task<string> GetAsync(string key, CancellationToken stopToken = default);

    Task SetAsync(string key, string value, CancellationToken stopToken = default);

    Task LoadSettingsAsync(CancellationToken stopToken = default);

    Task SaveSettingsAsync(CancellationToken stopToken = default);
  }
}
