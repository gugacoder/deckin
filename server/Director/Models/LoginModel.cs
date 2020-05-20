using System;
using Dapper;
using Director.Connectors;
using Director.Domain;
using Keep.Paper.Helpers;
using Keep.Tools;
using Keep.Tools.Sequel;
using Keep.Tools.Sequel.Runner;
using Microsoft.EntityFrameworkCore;

namespace Director.Models
{
  class X : Dapper.DynamicParameters
  {
  }

  public class LoginModel
  {
    private readonly DirectorDbContext dbDirector;

    public LoginModel(DirectorDbContext dbDirector)
    {
      this.dbDirector = dbDirector;
    }

    public Ret<object> Autenticar(Login login)
    {
      using var cn = dbDirector.Database.GetDbConnection();
      var usuario =
        @"select TBusuario.DFid_usuario
               , TBusuario.DFnome_usuario
               , TBnivel.DFdescricao
               , case when
                   dbo.fn_decript(coalesce(TBopcoes.DFvalor, TBusuario.DFsenha))
                       = @password
                   then 1
                   else 0
                 end as DFautenticado
            from TBusuario
           inner join TBnivel
                   on TBnivel.DFid_nivel_usuario = TBusuario.DFnivel_usuario
            left join TBopcoes
                   on TBopcoes.DFcodigo = 391
                  and TBusuario.DFnivel_usuario = 99
           where DFnome_usuario = @username"
          .AsSql()
          .Set(login)
          .SelectOne(cn, (int id, string nome, string papel, bool autenticado) =>
              new { id, nome, papel, autenticado });

      if (usuario?.autenticado != true)
        return null;

      var jwtBuilder = new JwtTokenBuilder();
      jwtBuilder.AddUsername(usuario.nome);

      var jwtToken = jwtBuilder.BuildJwtToken();
      return jwtToken;
    }

    object Cast(int id, int name) => new { };
  }
}
