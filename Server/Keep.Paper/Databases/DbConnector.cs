using System;
using System.ComponentModel;
using System.Data.Common;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;
using Keep.Paper.Security;
using Keep.Tools;
using Keep.Tools.Data;
using Keep.Tools.Reflection;
using Keep.Tools.Sequel.Runner;
using Microsoft.Extensions.Configuration;

namespace Keep.Paper.Databases
{
  public class DbConnector<T> : IDbConnector<T>
    where T : class, IDbContext
  {
    private readonly IConfiguration configuration;
    private readonly IUserContext userContext;

    public T Context { get; }

    public DbConnector(IConfiguration configuration, IUserContext userContext,
      IServiceProvider serviceProvider)
    {
      this.configuration = configuration;
      this.userContext = userContext;
      this.Context = serviceProvider.Instantiate<T>();
    }

    public ConnectionString GetConnectionString()
    {
      var domain = userContext.User.Domain ?? "Default";

      var type = typeof(T);
      var attr = type._Attribute<ConnectionNameAttribute>();
      var name = attr?.ConnectionName ?? typeof(T).Name;

      string connectionString = null;
      string provider = null;

      var paths = new[]{
        $"Domains:{domain}:ConnectionStrings:{name}",
        $"ConnectionStrings:{name}@{domain}",
        $"ConnectionStrings:{name}"
      };

      foreach (var path in paths)
      {
        connectionString =
          configuration[$"{path}:ConnectionString"] ??
          configuration[$"{path}"];

        if (connectionString != null)
        {
          provider = configuration[$"{path}:Provider"] ?? DataProviders.SqlServer;
          break;
        }
      }

      if (connectionString == null)
        throw new Exception($"Não existe um string de conexão configurada sob o nome {name} para o domínio {domain}.");

      return new ConnectionString(name, connectionString, provider);
    }

    public DbConnection Connect()
    {
      return ConnectAsync(default).Await();
    }

    public async Task<DbConnection> ConnectAsync(CancellationToken stopToken)
    {
      var cnString = GetConnectionString();
      var factory = DataProviders.CreateProviderFactory(cnString.Provider);
      var cn = factory.CreateConnection();
      cn.ConnectionString = cnString;

      await cn.OpenAsync(stopToken);
      await cn.SetDefaultOptionsAsync(stopToken);
      await Context.ConfigureAsync(cn, stopToken);

      return cn;
    }
  }
}
