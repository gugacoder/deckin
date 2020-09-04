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
  /// Gerenciador central de strings de conexão do Paper.
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

    private const string DefaultProvider = "System.Data.SqlClient";

    private readonly HashMap<string> providers;
    private readonly HashMap<string> strings;
    private readonly HashMap<HashMap<string>> templates;

    private readonly IConfiguration configuration;

    public DbConnector(IConfiguration configuration)
    {
      this.providers = new HashMap<string>();
      this.strings = new HashMap<string>();
      this.templates = new HashMap<HashMap<string>>();
      this.configuration = configuration;
    }

    public DbConnection Connect(string name)
    {
      return ConnectAsync(name,
        default, default, default, default, default, default).Await();
    }

    public DbConnection Connect(string name,
      string server = null, string database = null, int? port = null,
      string username = null, string password = null)
    {
      return ConnectAsync(name, default, server, database, port, username, password)
        .Await();
    }

    public async Task<DbConnection> ConnectAsync(string name,
      CancellationToken stopToken = default,
      string server = null, string database = null, int? port = null,
      string username = null, string password = null)
    {
      var connectionString = GetConnectionString(name,
        server, database, port, username, password);

      if (connectionString == null)
        throw new Exception(
          $"A base de dados do `{name}` não está configurada no sistema.");

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

    public string GetProvider(string name)
    {
      return providers[name] ??=
        (configuration[$"ConnectionStrings:{name}:Provider"] ?? DefaultProvider);
    }

    public void SetProvider(string name, string provider)
    {
      providers[name] = provider;
    }

    public string GetConnectionString(string name)
    {
      return GetConnectionString(name, null, null, null, null, null);
    }

    public string GetConnectionString(string name,
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

      if (server != null) template["server"] = server;
      if (port != null) template["port"] = port.ToString();
      if (database != null) template["database"] = database;
      if (username != null) template["username"] = username;
      if (password != null) template["password"] = password;

      return BuildConnectionString(template);
    }

    private string BuildConnectionString(HashMap<string> parameters)
    {
      if (!string.IsNullOrEmpty(parameters["pwd"]))
      {
        parameters["pwd"] = Api.Crypto.Decrypt(parameters["pwd"]);
      }
      return string.Join(";",
        parameters.Select(entry => $"{entry.Key}={entry.Value}"));
    }

    public void SetConnectionString(string name, string connectionString)
    {
      var parameters = SplitParameters(connectionString);
      templates[name] = parameters;
      strings[name] = BuildConnectionString(parameters);
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
          return "server";

        case "database":
        case "default catalog":
          return "database";

        case "user id":
        case "uid":
          return "uid";

        case "password":
        case "pwd":
          return "pwd";

        default:
          return key;
      }
    }
  }
}
