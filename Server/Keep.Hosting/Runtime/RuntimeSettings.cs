using System;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using Keep.Tools.Collections;
using Keep.Tools.Data;

namespace Keep.Hosting.Runtime
{
  public class RuntimeSettings : IRuntimeSettings
  {
    private readonly TaskCompletionSource<bool> signaler;
    private readonly HashMap<string> settings;
    private readonly string connectionString;
    private readonly string provider;

    public RuntimeSettings(string connectionString,
      string provider = DataProviders.SqlServer)
    {
      this.signaler = new TaskCompletionSource<bool>();
      this.settings = new HashMap<string>();
      this.connectionString = connectionString;
      this.provider = provider;
    }

    public async Task AwaitAvailability()
    {
      await signaler.Task;
    }

    public async Task<string> GetAsync(string key, CancellationToken stopToken = default)
    {
      var value = settings[key];
      return await Task.FromResult(value);
    }

    public async Task SetAsync(string key, string value, CancellationToken stopToken = default)
    {
      settings[key] = value;
      await Task.CompletedTask;
    }

    public async Task LoadSettingsAsync(CancellationToken stopToken = default)
    {
      using var cn = await ConnectAsync(stopToken);
      signaler.SetResult(true);
    }

    public async Task SaveSettingsAsync(CancellationToken stopToken = default)
    {
      using var cn = await ConnectAsync(stopToken);
      await Task.CompletedTask;
    }

    private async Task<DbConnection> ConnectAsync(CancellationToken stopToken)
    {
      var factory = DataProviders.CreateProviderFactory(provider);
      var connection = factory.CreateConnection();
      connection.ConnectionString = connectionString;
      await connection.OpenAsync(stopToken);
      return connection;
    }
  }
}
