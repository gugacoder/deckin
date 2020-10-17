using System;
using System.Linq;
using System.Threading;
using AppSuite.Servicos;
using Keep.Paper.Api;
using Types = Keep.Paper.Api.Types;
using Keep.Tools;
using Keep.Tools.Collections;
using Microsoft.AspNetCore.Authorization;
using static AppSuite.Servicos.ServicoDeAuditoria;

namespace Mercadologic.Replicacao.Paginas
{
  [Expose]
  [HomePaper]
  //[AllowAnonymous]
  public class PaginaDeMonitoramento : AbstractPaper
  {
    private const int MaxLimit = 10000;
    private readonly ServicoDeAuditoria auditoria;

    public class Filtro
    {
      /*
      public Var<DateTime?> Data { get; set; } = new Var<DateTime?>(
        new Range<DateTime?>(
          DateTime.Today.AddDays(-1),
          DateTime.Today
        ));

      public Var<string> Origem { get; set; }

      public Var<string> Evento { get; set; }

      public Var<string> Mensagem { get; set; }
      */

      public Filtro()
      {
        De = DateTime.Today.AddDays(-1);
        Ate = DateTime.Today;
      }

      public DateTime? De { get; set; }
      public DateTime? Ate { get; set; }
      public string Origem { get; set; }
      public string Evento { get; set; }
      public string Mensagem { get; set; }
    }

    public PaginaDeMonitoramento(ServicoDeAuditoria auditoria)
    {
      this.auditoria = auditoria;
    }

    public Types.Entity Index(Filtro filtro, Pagination pagination)
    {
      int limit = (pagination?.Limit == null)
        ? (int)PageLimit.UpTo50
        : (int)pagination.Limit;

      var events = auditoria.Find(mapa => mapa
        .SelectMany(item => item.Value.Find(eventos => eventos
          .NotNull()
          .Where(evento =>
            filtro.De == null ||
            evento.Data.CompareTo(filtro.De) >= 0
          )
          .Where(evento =>
            filtro.Ate == null ||
            evento.Data.CompareTo(filtro.Ate.Value.AddDays(1)) < 0
          )
          .Where(evento =>
            string.IsNullOrEmpty(filtro.Origem) ||
            evento.Origem.ContainsIgnoreCase(filtro.Origem)
          )
          .Where(evento =>
            string.IsNullOrEmpty(filtro.Evento) ||
            evento.Nome.ContainsIgnoreCase(filtro.Evento)
          )
          .Where(evento =>
            string.IsNullOrEmpty(filtro.Mensagem) ||
            evento.Mensagem.ContainsIgnoreCase(filtro.Mensagem)
          )
          .OrderByDescending(evento => evento.Data)
          .Take(limit > 0 ? limit : MaxLimit)
        ))
        .OrderByDescending(evento => evento.Data)
        .Take(limit > 0 ? limit : MaxLimit)
        .Select(evento =>
          new Types.Entity
          {
            Data = new Types.Data(evento)
          }
        )
      );

      return new Types.Action
      {
        Props = new Types.GridView
        {
          Title = "Eventos do Sistema",
          AutoRefresh = 2, // segundos
          Pagination = new Pagination
          {
            Limit = limit
          }
        },

        Fields = new Types.FieldCollection
        {
          new Types.Field
          {
            Props = new Types.Header
            {
              Name = nameof(Evento.Id).ToCamelCase(),
              Hidden = true
            }
          },
          new Types.Field
          {
            Props = new Types.Header
            {
              Name = nameof(Evento.Origem).ToCamelCase()
            }
          },
          new Types.Field
          {
            Props = new Types.Header
            {
              Name = nameof(Evento.Nome).ToCamelCase(),
              Title = "Evento"
            }
          },
          new Types.Field
          {
            Props = new Types.Header
            {
              Name = nameof(Evento.Data).ToCamelCase()
            }
          },
          new Types.Field
          {
            Props = new Types.Header
            {
              Name = nameof(Evento.Mensagem).ToCamelCase()
            }
          },
          new Types.Field
          {
            Props = new Types.Header
            {
              Name = nameof(Evento.Nivel).ToCamelCase(),
              Hidden = true,
              UseForStyle = true
            }
          }
        },

        Actions = new Types.ActionCollection
        {
          new Types.Action
          {
            Data = new Types.Data(filtro),
            Props = new Types.View {
              Name = "filter",
              Title = "Filtro"
            },
            Fields = new Types.FieldCollection
            {
              //new
              //{
              //  Kind = FieldKind.Date,
              //  View = new
              //  {
              //    Name = 
              //    Range = true,
              //  }
              //},
              new Types.Field
              {
                Props = new Types.DateWidget
                {
                  Name = nameof(Filtro.De).ToCamelCase(),
                  Title = "De"
                }
              },
              new Types.Field
              {
                Props = new Types.DateWidget
                {
                  Name = nameof(Filtro.Ate).ToCamelCase(),
                  Title = "Até"
                }
              },
              new Types.Field
              {
                Props = new Types.TextWidget
                {
                  Name = nameof(Filtro.Origem).ToCamelCase()
                }
              },
              new Types.Field
              {
                Props = new Types.TextWidget
                {
                  Name = nameof(Filtro.Evento).ToCamelCase()
                }
              },
              new Types.Field
              {
                Props = new Types.TextWidget
                {
                  Name = nameof(Filtro.Mensagem).ToCamelCase()
                }
              }
            }
          }
        },

        Embedded = new Types.EntityCollection(events),

        Links = new Types.LinkCollection
        {
          new Types.Link
          {
            Rel = Rel.Self,
            Href = Href.To(HttpContext, GetType(), nameof(Index)),
          },
          new Types.Link
          {
            Rel = Rel.Workspace,
            Href = Href.To(HttpContext, typeof(AreaDeTrabalho))
          }
        }
      };
    }
  }
}