using System;
using System.Data.Common;
using System.Data.SqlClient;
using Director.Configuracoes;
using Keep.Paper.Services;

namespace Director.Conectores
{
  public class DbDirector
  {
    private readonly ICommonSettings settings;

    public DbDirector(ICommonSettings settings)
    {
      this.settings = settings;
    }

    public DbConnection CriarConexao()
    {
      var stringDeConexao = settings.Get(Chaves.StringsDeConexao.Director);
      if (string.IsNullOrEmpty(stringDeConexao))
        throw new Exception("A base de dados do Director não está configurada no sistema.");

      return new SqlConnection(stringDeConexao);
    }
  }
}
