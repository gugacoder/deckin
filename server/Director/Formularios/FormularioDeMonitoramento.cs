using System;
using System.Linq;
using System.Threading;
using Director.Servicos;
using Keep.Paper.Api;
using Keep.Tools;
using Keep.Tools.Collections;
using Microsoft.AspNetCore.Authorization;
using static Director.Servicos.ServicoDeAuditoria;

namespace Director.Formularios
{
  [Expose]
  [HomePaper]
  //[AllowAnonymous]
  public class FormularioDeMonitoramento : BasePaper
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

    public FormularioDeMonitoramento(ServicoDeAuditoria auditoria)
    {
      this.auditoria = auditoria;
    }

    public object Index(Filtro filtro, Pagination pagination)
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
        .Select(evento => new { Data = evento })
        .Take(limit > 0 ? limit : MaxLimit)
      );

      return new
      {
        Kind = Kind.Paper,

        View = new
        {
          Title = "Monitoramento de Eventos do Sistema",
          Design = Design.Grid,
          AutoRefresh = 1, // segundos
          FilterHidden = true,
          Page = new { limit }
        },

        Fields = new object[]
        {
          new {
            Data = new {
              Name = nameof(Evento.Id).ToCamelCase()
            },
            View = new {
              Hidden = true
            }
          },
          new {
            Data = new {
              Name = nameof(Evento.Origem).ToCamelCase()
            }
          },
          new {
            Data = new {
              Name = nameof(Evento.Nome).ToCamelCase()
            },
            View = new {
              Title = "Evento"
            }
          },
          new {
            Data = new {
              Name = nameof(Evento.Data).ToCamelCase()
            }
          },
          new {
            Data = new {
              Name = nameof(Evento.Mensagem).ToCamelCase()
            }
          },
          new {
            Data = new {
              Name = nameof(Evento.Nivel).ToCamelCase()
            },
            View = new {
              Hidden = true,
              UseForStyle = true
            }
          }
        },

        Actions = new
        {
          Filter = new
          {
            Data = filtro,
            Fields = new object[]
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
              new
              {
                Kind = FieldKind.Date,
                View = new
                {
                  Name = nameof(Filtro.De).ToCamelCase(),
                  Title = "De"
                }
              },
              new
              {
                Kind = FieldKind.Date,
                View = new
                {
                  Name = nameof(Filtro.Ate).ToCamelCase(),
                  Title = "Até"
                }
              },
              new
              {
                Kind = FieldKind.Text,
                View = new
                {
                  Name = nameof(Filtro.Origem).ToCamelCase()
                }
              },
              new
              {
                Kind = FieldKind.Text,
                View = new
                {
                  Name = nameof(Filtro.Evento).ToCamelCase()
                }
              },
              new
              {
                Kind = FieldKind.Text,
                View = new
                {
                  Name = nameof(Filtro.Mensagem).ToCamelCase()
                }
              }
            }
          }
        },

        Embedded = events,

        Links = new
        {
          Self = Href.To(HttpContext, GetType(), nameof(Index))
        }
      };
    }

  }
}
