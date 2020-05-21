using System;
using System.Threading.Tasks;
using Dapper;
using Director.Connectors;
using Director.Domain.Aut;
using Keep.Paper.Security;
using Keep.Tools;
using Keep.Tools.Sequel;
using Keep.Tools.Sequel.Runner;
using Microsoft.EntityFrameworkCore;

namespace Director.Models
{
  public class LoginModel
  {
    private readonly DirectorDbContext dbDirector;

    public LoginModel(DirectorDbContext dbDirector)
    {
      this.dbDirector = dbDirector;
    }

    public async Task<Identidade> AutenticarAsync(Credencial credencial)
    {
      using var cn = dbDirector.Database.GetDbConnection();

      var identidade = await
        @"select TBusuario.DFid_usuario as IdUsuario
               , TBusuario.DFnome_usuario as Usuario
               , TBnivel.DFid_nivel_usuario as IdNivel
               , TBnivel.DFdescricao as Nivel
               , case when
                   dbo.fn_decript(coalesce(TBopcoes.DFvalor, TBusuario.DFsenha))
                       = @senha
                   then 1
                   else 0
                 end as Autenticado
            from TBusuario
           inner join TBnivel
                   on TBnivel.DFid_nivel_usuario = TBusuario.DFnivel_usuario
            left join TBopcoes
                   on TBopcoes.DFcodigo = 391
                  and TBusuario.DFnivel_usuario = 99
           where DFnome_usuario = @usuario"
          .AsSql()
          .Set(credencial)
          .SelectOneAsync<Identidade>(cn);

      return identidade;
    }
  }
}
