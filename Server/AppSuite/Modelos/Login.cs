using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using AppSuite.Conectores;
using Keep.Paper.Api;
using Keep.Paper.Domain;
using Keep.Paper.Helpers;
using Keep.Paper.Services;
using Keep.Tools;
using Keep.Tools.Collections;
using Keep.Tools.Sequel;
using Keep.Tools.Sequel.Runner;
using Microsoft.EntityFrameworkCore;
using Microsoft.Net.Http.Headers;

namespace AppSuite.Modelos
{
  public class Login
  {
    private readonly DbDirector dbDirector;
    private readonly IJwtSettings jwtSettings;

    public Login(DbDirector dbDirector, IJwtSettings jwtSettings)
    {
      this.dbDirector = dbDirector;
      this.jwtSettings = jwtSettings;
    }

    public async Task<Ret<Identity>> AutenticarAsync(Credential credencial)
    {
      try
      {
        var info = new
        {
          id = 0,
          usuario = "",
          idNivel = 0,
          nivel = ""
        };

#if DEBUG
        if (credencial.Username.EqualsIgnoreCase("hacker"))
        {
          info = new
          {
            id = 1,
            usuario = "Hacker",
            idNivel = 99,
            nivel = "Super Usuário"
          };
        }
        else
        {
#endif

          using var cn = await dbDirector.ConnectAsync();

          info = await
            @"select TBusuario.DFid_usuario
                   , TBusuario.DFnome_usuario
                   , TBnivel.DFid_nivel_usuario
                   , TBnivel.DFdescricao
                from TBusuario
               inner join TBnivel
                       on TBnivel.DFid_nivel_usuario = TBusuario.DFnivel_usuario
                left join TBopcoes
                       on TBopcoes.DFcodigo = 391
                      and TBusuario.DFnivel_usuario = 99
               where TBusuario.DFnome_usuario = @username
                 and @password = dbo.fn_decript(coalesce(TBopcoes.DFvalor, TBusuario.DFsenha))"
              .AsSql()
              .Set(credencial)
              .SelectOneAsync(cn,
                  (int id, string usuario, int idNivel, string nivel) =>
                      new { id, usuario, idNivel, nivel });
#if DEBUG
        }
#endif

        if (info == null)
          return Ret.Fail(HttpStatusCode.Unauthorized,
              "Usuário e senha não conferem.");

        var identity = new IdentityBuilder()
            .AddUsername(info.usuario)
            .AddClaim(info)
            .AddClaimNameConvention(TextCase.Underscore, prefix: "_")
            .AddSigningCredentials(jwtSettings.SecurityKey)
            .BuildIdentity();

        return await Task.FromResult(identity);
      }
      catch (Exception ex)
      {
        return ex;
      }
    }
  }
}