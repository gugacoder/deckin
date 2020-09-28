using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Director.Conectores;
using Director.Dominio.dbo;
using Keep.Paper.Api;
using Types = Keep.Paper.Api.Types;
using Keep.Tools;
using Keep.Tools.Collections;
using Keep.Tools.Sequel;
using Keep.Tools.Sequel.Runner;

namespace Director.Paginas
{
  [Expose]
  public class PaginaDeEmpresaMercadologic : AbstractPaper
  {
    private readonly DbDirector dbDirector;

    public PaginaDeEmpresaMercadologic(DbDirector dbDirector)
    {
      this.dbDirector = dbDirector;
    }

    public async Task<Types.Action> Index(Filtro filtro, Pagination pagination)
    {
      pagination.Limit ??= (int)PageLimit.UpTo50;

      using var cnDirector = await dbDirector.ConnectAsync();

      SequelTracer.TraceQuery = sql => Debug.WriteLine(sql);

      var empresas = await @"
        select TBempresa_mercadologic.DFcod_empresa
             , TBempresa.DFnome_fantasia
             , TBempresa_mercadologic.DFprovider
             , TBempresa_mercadologic.DFdriver
             , TBempresa_mercadologic.DFservidor
             , TBempresa_mercadologic.DFporta
             , TBempresa_mercadologic.DFdatabase
             , TBempresa_mercadologic.DFusuario
          from TBempresa_mercadologic
         inner join TBempresa
                 on TBempresa.DFcod_empresa = TBempresa_mercadologic.DFcod_empresa
         where TBempresa_mercadologic.DFcod_empresa matches if set @DFcod_empresa
           and TBempresa.DFnome_fantasia matches if set @DFnome_fantasia"
        .AsSql()
        .Set(filtro)
        .Echo()
        .SelectAsync<TBempresa_mercadologic>(cnDirector);

      return new Types.Action<Types.GridView>
      {
        Props = new Types.GridView
        {
          Title = "Empresa do Mercadologic",
          //AutoRefresh = 1, // segundos
          Pagination = pagination,
        },
        Embedded = new Types.EntityCollection(empresas.Select(empresa =>
          new Types.Entity
          {
            Data = new Types.Data(empresa)
          }
        )),
        Fields = new Types.FieldCollection
        {
          new Types.Field
          {
            Props = new Types.IntWidget
            {
              Name = "DFcod_empresa",
              Title = "Empresa"
            }
          },
          new Types.Field
          {
            Props = new Types.TextWidget
            {
              Name = "DFnome_fantasia",
              Title = "Nome da Empresa"
            }
          },
          new Types.Field
          {
            Props = new Types.TextWidget
            {
              Name = "DFprovider",
              Title = "Provider"
            }
          },
          new Types.Field
          {
            Props = new Types.TextWidget
            {
              Name = "DFdriver",
              Title = "Driver"
            }
          },
          new Types.Field
          {
            Props = new Types.TextWidget
            {
              Name = "DFservidor",
              Title = "Servidor"
            }
          },
          new Types.Field
          {
            Props = new Types.IntWidget
            {
              Name = "DFporta",
              Title = "Porta"
            }
          },
          new Types.Field
          {
            Props = new Types.TextWidget
            {
              Name = "DFdatabase",
              Title = "Database"
            }
          },
          new Types.Field
          {
            Props = new Types.TextWidget
            {
              Name = "DFusuario",
              Title = "Usuário",
              Username = true
            }
          }
        },

        Actions = new Types.ActionCollection
        {
          new Types.Action
          {
            Props = new Types.View
            {
              Name = ActionName.Filter
            },
            Fields = new Types.FieldCollection
            {
              new Types.Field
              {
                Props = new Types.IntWidget
                {
                  Name = "DFcod_empresa",
                  Title = "Empresa"
                }
              },
              new Types.Field
              {
                Props = new Types.TextWidget
                {
                  Name = "DFnome_fantasia",
                  Title = "Nome da Empresa"
                }
              }
            },
            Links = new Types.LinkCollection
            {
              new Types.Link
              {
                Rel = Rel.Action,
                Title = "Filtro",
                Href = Href.To(HttpContext, GetType())
              }
            }
          }
        },

        Links = new Types.LinkCollection
        {
          new Types.Link
          {
            Rel = Rel.Self,
            Href = Href.To(HttpContext, GetType())
          },
          new Types.Link
          {
            Rel = Rel.Workspace,
            Href = Href.To(HttpContext, typeof(AreaDeTrabalho))
          }
        }
      };
    }

    public class Filtro
    {
      public int? DFcod_empresa { get; set; }
      public string DFnome_fantasia { get; set; }
    }
  }
}
