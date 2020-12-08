using System;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using Keep.Hosting.Auth;
using Keep.Tools;
using Keep.Tools.Collections;
using Keep.Tools.Data;

namespace Keep.Hosting.Data
{
  //
  // In-Memory Sqlite DB
  // - https://www.sqlite.org/inmemorydb.html
  //
  public class LocalData : DbConnector<LocalData>
  {
    private static readonly object @lock = new object();
    private static readonly HashMap<DbConnection> connections =
      new HashMap<DbConnection>();

    private readonly IUserContext userContext;
    private readonly IServiceProvider serviceProvider;

    // Carregadores de dados. O carregador de dados recebe os parâmetros:
    //  -   Coleção de serviços.
    //  -   Conexão estabelecida com a base de dados SQLite.
    private static readonly
      HashMap<Action<IServiceProvider, DbConnection>> dataLoaders;

    static LocalData()
    {
      dataLoaders = new HashMap<Action<IServiceProvider, DbConnection>>();
    }

    public LocalData(IDbConnector dbConnector, IUserContext userContext,
      IServiceProvider serviceProvider)
      : base(nameof(LocalData), dbConnector)
    {
      this.userContext = userContext;
      this.serviceProvider = serviceProvider;
      this.SetProvider(DataProviders.Sqlite);
      this.SetConnectionString("Data Source=;Mode=Memory;Cache=Shared");
    }

    public void RegisterDataLoader(string databaseName,
      Action<DbConnection> dataLoader)
    {
      dataLoaders[databaseName] = (provider, cn) => dataLoader(cn);
    }

    public void RegisterDataLoader(string databaseName,
      Action<IServiceProvider, DbConnection> dataLoader)
    {
      dataLoaders[databaseName] = dataLoader;
    }

    public override DbConnection Connect(
      string server = null,
      string database = null,
      int? port = null,
      string username = null,
      string password = null)
    {
      return ConnectAsync(
        server,
        database,
        port,
        username,
        password,
        default
      ).Await();
    }

    public override async Task<DbConnection> ConnectAsync(
      string server = null,
      string database = null,
      int? port = null,
      string username = null,
      string password = null,
      CancellationToken stopToken = default)
    {
      // Conceito:
      //    LocalData é mantido em bases do SQLite em memória.
      //    O SQLite mantém uma mesma base ativa para múltiplas conexões
      //    enquanto houver pelo menos uma conexão ativa, depois disto a
      //    base em memória é descartada e uma nova é criada na próxima
      //    conexão estabelecida.
      //
      //    Por causa disto estamos mantendo aberta a primeira conexão
      //    estabelecida, enquanto o sistema estiver em execução.

      // O nome do objeto é prefixado com o domínio do usuário
      // para compartamentalizar a base em memória em ambiente
      // multi-inquilino (Multitenant).
      var dataSource = $"{userContext.User.Domain}__{server}";

      var cn =
        await base.ConnectAsync(
          dataSource, database, port, username, password, stopToken);

      lock (@lock)
      {
        if (!connections.ContainsKey(dataSource))
        {
          // Guardando uma conexão aberta para segurar a base de dados
          // em memória enquanto o sistema estiver aberto.
          // SQLite mantém a base em memória ativa enquanto houver pelo menos
          // uma conexão estabelecida.
          connections[dataSource] = cn;

          var loader = dataLoaders[server];
          loader?.Invoke(serviceProvider, cn);

          // Guardamos a conexão, uma nova será criada e retornada ao chamador.
          cn = null;
        }
      }

      return cn ??
        await base.ConnectAsync(
          dataSource, database, port, username, password, stopToken);
    }
  }
}
