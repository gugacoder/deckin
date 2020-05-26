﻿using System;
using System.Net;
using System.Threading.Tasks;
using Dapper;
using Director.Connectors;
using Keep.Paper.Domain;
using Keep.Paper.Helpers;
using Keep.Paper.Services;
using Keep.Tools;
using Keep.Tools.Collections;
using Keep.Tools.Sequel;
using Keep.Tools.Sequel.Runner;
using Microsoft.EntityFrameworkCore;
using Microsoft.Net.Http.Headers;

namespace Director.Models
{
  public class LoginModel
  {
    private readonly DirectorDbContext dbDirector;
    private readonly IJwtSettings jwtSettings;

    public LoginModel(DirectorDbContext dbDirector, IJwtSettings jwtSettings)
    {
      this.dbDirector = dbDirector;
      this.jwtSettings = jwtSettings;
    }

    public async Task<Identity> AutenticarAsync(Credential credencial)
    {
      using var cn = dbDirector.Database.GetDbConnection();

      var info = await
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

      if (info == null)
        return null;

      var identity = new IdentityBuilder()
          .AddUsername(info.usuario)
          .AddClaim(info)
          .AddClaimNameConvention(TextCase.Underscore, prefix: "_")
          .AddSigningCredentials(jwtSettings.SecurityKey)
          .BuildIdentity();

      return identity;
    }
  }
}