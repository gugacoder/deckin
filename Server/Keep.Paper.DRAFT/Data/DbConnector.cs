using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Keep.Paper.Api;
using Keep.Tools;
using Keep.Tools.Collections;
using Keep.Tools.Data;
using Microsoft.Extensions.Configuration;

namespace Keep.Paper.Data
{
  /// <summary>
  /// Gerenciador central de strings de conexão  Paper.
  /// 
  /// As strings de conexão podem ser configuradas no arquivo `appsettings.json`
  /// na forma:
  /// 
  ///   "ConnectionStrings": {
  ///     "Director": {
  ///       "Provider": "System.Data.SqlClient",
  ///       "ConnectionString": "server=...;database=...;uid=...;pwd=...;timeout=60;"
  ///     }
  ///   }
  /// </summary>
  public class DbConnector : IDbConnector
  {
    private const string ConnectionStringTemplate =
      "Server=;Database=;User ID=;Password=;Timeout=60;";

    private const string DefaultProvider = DataProviders.SqlServer;

    private const string Server = "Data Source";
    private const string Database = "Default Catalog";
    private const string Port = "port";
    private const string Username = "Uid";
    private const string Password = "Pwd";

    private readonly HashMap<string> providers;
    private readonly HashMap<string> strings;
    private readonly HashMap<IDbConnectorProxy> proxies;
    private readonly HashMap<HashMap<string>> templates;

    private readonly IConfiguration configuration;

    public DbConnector(IConfiguration configuration)
    {
      this.providers = new HashMap<string>();
      this.strings = new HashMap<string>();
      this.proxies = new HashMap<IDbConnectorProxy>();
      this.templates = new HashMap<HashMap<string>>();
      this.configuration = configuration;
    }

    internal void RegisterProxy(IDbConnectorProxy proxy)
    {
      proxies[proxy.Name] = proxy;
    }

    internal virtual DbConnection Connect(string name)
    {
      return ConnectAsync(name,
        default, default, default, default, default, default).Await();
    }

    internal virtual DbConnection Connect(string name,
      string server = null, string database = null, int? port = null,
      string username = null, string password = null)
    {
      return ConnectAsync(name, server, database, port, username, password, default)
        .Await();
    }

    internal virtual async Task<DbConnection> ConnectAsync(string name,
      string server = null, string database = null, int? port = null,
      string username = null, string password = null,
      CancellationToken stopToken = default)
    {
      var connectionString = GetConnectionString(name,
        server, database, port, username, password);

      if (connectionString == null)
        throw new Exception(
          $"A base de dados  `{name}` não está configurada no sistema.");

      var provider = GetProvider(name);
      var factory = DataProviders.CreateProviderFactory(provider);

      var connection = factory.CreateConnection();
      try
      {
        connection.ConnectionString = connectionString;
        await connection.OpenAsync();
      }
      catch
      {
        connection.TryDispose();
        throw;
      }

      return connection;
    }

    internal virtual string GetProvider(string name)
    {
      return providers[name] ??=
        (configuration[$"ConnectionStrings:{name}:Provider"] ?? DefaultProvider);
    }

    internal virtual void SetProvider(string name, string provider)
    {
      providers[name] = provider;
    }

    internal virtual string GetConnectionString(string name)
    {
      return GetConnectionString(name, null, null, null, null, null);
    }

    internal virtual string GetConnectionString(string name,
      string server = null, string database = null, int? port = null,
      string username = null, string password = null)
    {
      if (!strings.ContainsKey(name))
      {
        var @string = configuration[$"ConnectionStrings:{name}:ConnectionString"];
        if (string.IsNullOrEmpty(@string))
          throw new Exception(
            $"A base de dados `{name}` não está configurada no sistema.");

        SetConnectionString(name, @string);
      }

      var useTemplate =
        server != null ||
        database != null ||
        port != null ||
        username != null ||
        password != null;

      if (!useTemplate)
        return strings[name];

      var template = new HashMap<string>(templates[name]);

      if (server != null) template[Server] = server;
      if (port != null) template[Port] = port.ToString();
      if (database != null) template[Database] = database;
      if (username != null) template[Username] = username;
      if (password != null) template[Password] = password;

      return BuildConnectionString(template);
    }

    internal virtual void SetConnectionString(string name, string connectionString)
    {
      var parameters = SplitParameters(connectionString);
      templates[name] = parameters;
      strings[name] = BuildConnectionString(parameters);
    }

    private string BuildConnectionString(HashMap<string> parameters)
    {
      if (!string.IsNullOrEmpty(parameters[Password]))
      {
        parameters[Password] = Api.Crypto.Decrypt(parameters[Password]);
      }
      return string.Join(";",
        parameters.Select(entry => $"{entry.Key}={entry.Value}"));
    }

    private HashMap<string> SplitParameters(string connectionString)
    {
      connectionString ??= ConnectionStringTemplate;

      var entries =
        from parameter in connectionString.Split(";")
        let parts = parameter.Split('=')
        let key = CanonicalizeKey(parts.First())
        let value = string.Join("=", parts.Skip(1))
        where !string.IsNullOrEmpty(key)
        select new KeyValuePair<string, string>(key, value);

      var template = new HashMap<string>(entries);
      return template;
    }

    private string CanonicalizeKey(string key)
    {
      switch (key.ToLower())
      {
        case "server":
        case "data source":
        case "host":
          return Server;

        case "database":
        case "default catalog":
          return Database;

        case "user id":
        case "uid":
          return Username;

        case "password":
        case "pwd":
          return Password;

        default:
          return key;
      }
    }

    #region Implementação de IDbConnector

    DbConnection IDbConnector.Connect(string name, string server, string database, int? port, string username, string password)
    {
      var proxy = proxies[name];
      if (proxy != null)
        return proxy.Connect(server, database, port, username, password);
      else
        return Connect(name, server, database, port, username, password);
    }

    async Task<DbConnection> IDbConnector.ConnectAsync(string name, string server, string database, int? port, string username, string password, CancellationToken stopToken)
    {
      var proxy = proxies[name];
      if (proxy != null)
        return await proxy.ConnectAsync(server, database, port, username, password, stopToken);
      else
        return await ConnectAsync(name, server, database, port, username, password, stopToken);
    }

    string IDbConnector.GetProvider(string name)
    {
      var proxy = proxies[name];
      if (proxy != null)
        return proxy.GetProvider();
      else
        return GetProvider(name);
    }

    void IDbConnector.SetProvider(string name, string provider)
    {
      var proxy = proxies[name];
      if (proxy != null)
        proxy.SetProvider(provider);
      else
        SetProvider(name, provider);
    }

    string IDbConnector.GetConnectionString(string name, string server, string database, int? port, string username, string password)
    {
      var proxy = proxies[name];
      if (proxy != null)
        return proxy.GetConnectionString(server, database, port, username, password);
      else
        return GetConnectionString(name, server, database, port, username, password);
    }

    void IDbConnector.SetConnectionString(string name, string connectionString)
    {
      var proxy = proxies[name];
      if (proxy != null)
        proxy.SetConnectionString(connectionString);
      else
        SetConnectionString(name, connectionString);
    }

    #endregion
  }
}
