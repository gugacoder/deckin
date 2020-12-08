using System;
using System.Threading;
using System.Threading.Tasks;

namespace Keep.Hosting.Hosting
{
  public interface ISettings
  {
    Task<string> GetAsync(string key, CancellationToken stopToken = default);

    Task SetAsync(string key, string value, CancellationToken stopToken = default);
  }
}
