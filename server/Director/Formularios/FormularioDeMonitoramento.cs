using System;
using System.Linq;
using System.Threading;
using Director.Servicos;
using Keep.Paper.Api;
using Keep.Tools;
using Keep.Tools.Collections;
using static Director.Servicos.ServicoDeAuditoria;

namespace Director.Formularios
{
  [Expose]
  [HomePaper]
  public class FormularioDeMonitoramento : BasePaper
  {
    private const int MaxLimit = 10000;
    private readonly ServicoDeAuditoria auditoria;

    public FormularioDeMonitoramento(ServicoDeAuditoria auditoria)
    {
      this.auditoria = auditoria;
    }

    public object Index(Pagination pagination)
    {
      Thread.Sleep(1000);

      int limit = (pagination?.Limit == null)
        ? (int)PageLimit.UpTo50
        : (int)pagination.Limit;

      return new
      {
        Kind = Kind.Paper,

        View = new
        {
          Title = "Monitoramento de Eventos do Sistema",
          Design = Design.Grid,
          AutoRefresh = 1, // segundos
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

        Embedded = auditoria.Find(mapa => mapa
          .SelectMany(item => item.Value.Find(eventos => eventos
            .NotNull()
            .OrderByDescending(evento => evento.Data)
            .Take(limit > 0 ? limit : MaxLimit)
          ))
          .OrderByDescending(evento => evento.Data)
          .Select(evento => new { Data = evento })
          .Take(limit > 0 ? limit : MaxLimit)
        ),

        Links = new
        {
          Self = Href.To(HttpContext, GetType(), nameof(Index))
        }
      };
    }

  }
}
