using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Director.Conectores;
using Director.Dominio.dbo;
using Keep.Paper.Api;
using Keep.Tools;
using Keep.Tools.Collections;
using Keep.Tools.Sequel;
using Keep.Tools.Sequel.Runner;
using Keep.Paper.Api.Types;

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

    public async Task<Entity> Index(Filtro filtro, Pagination pagination)
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

      return new Entity
      {
        Kind = Kind.Paper,

        View = new View
        {
          Title = "Empresa do Mercadologic",
          Design = new GridDesign
          {
            //AutoRefresh = 1, // segundos
            Pagination = pagination
          }
        },

        Embedded = empresas.ToCollection(empresa => new Entity
        {
          Data = empresa
        }),

        Fields = new Collection<Field>
        {
          new Field
          {
            Kind = FieldKind.Number,
            View = new View
            {
              Name = "DFcod_empresa",
              Title = "Empresa",
            }
          },
          new Field
          {
            Kind = FieldKind.Text,
            View = new View
            {
              Name = "DFnome_fantasia",
              Title = "Nome da Empresa",
            }
          },
          new Field
          {
            Kind = FieldKind.Text,
            View = new View
            {
              Name = "DFprovider",
              Title = "Provider",
            }
          },
          new Field
          {
            Kind = FieldKind.Text,
            View = new View
            {
              Name = "DFdriver",
              Title = "Driver",
            }
          },
          new Field
          {
            Kind = FieldKind.Text,
            View = new View
            {
              Name = "DFservidor",
              Title = "Servidor",
            }
          },
          new Field
          {
            Kind = FieldKind.Number,
            View = new View
            {
              Name = "DFporta",
              Title = "Porta",
            }
          },
          new Field
          {
            Kind = FieldKind.Text,
            View = new View
            {
              Name = "DFdatabase",
              Title = "Database",
            }
          },
          new Field
          {
            Kind = FieldKind.Username,
            View = new View
            {
              Name = "DFusuario",
              Title = "Usuário",
            }
          }
        },

        Actions = new Collection<Keep.Paper.Api.Types.Action>
        {
          new Keep.Paper.Api.Types.Action
          {
            View = new View
            {
              Name = ActionName.Filter
            },
            Fields = new Collection<Field>
            {
              new Field
              {
                Kind = FieldKind.Number,
                View = new View
                {
                  Name = "DFcod_empresa",
                  Title = "Empresa"
                }
              },
              new Field
              {
                Kind = FieldKind.Text,
                View = new View
                {
                  Name = "DFnome_fantasia",
                  Title = "Nome da Empresa"
                }
              }
            },
            Links = new Collection<Link>
            {
              new Link
              {
                Rel = Rel.Action,
                Title = "Filtro",
                Href = Href.To(HttpContext, GetType())
              }
            }
          }
        },

        Links = new Collection<Link>
        {
          new Link
          {
            Rel = Rel.Self,
            Href = Href.To(HttpContext, GetType())
          },
          new Link
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
