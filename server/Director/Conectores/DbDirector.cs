using System;
using System.Data.Common;
using System.Data.SqlClient;

namespace Director.Conectores
{
  public class DbDirector
  {
    private readonly string stringDeConexao;

    public DbDirector(string stringDeConexao)
    {
      this.stringDeConexao = stringDeConexao;
    }

    public DbConnection CriarConexao()
    {
      return new SqlConnection(stringDeConexao);
    }
  }
}
