using System;
using System.Data.Common;
using System.Data.SqlClient;
using Keep.Paper.Api;
using Microsoft.Extensions.Configuration;

namespace Director.Conectores
{
  public class DbDirector
  {
    private readonly IConfiguration configuration;

    public DbDirector(IConfiguration configuration)
    {
      this.configuration = configuration;
    }

    public DbConnection CriarConexao()
    {
      var stringDeConexao =
        configuration["ConnectionStrings:Director:ConnectionString"];

      if (string.IsNullOrEmpty(stringDeConexao))
        throw new Exception("A base de dados do Director não está configurada no sistema.");

      return new SqlConnection(stringDeConexao);
    }
  }
}
