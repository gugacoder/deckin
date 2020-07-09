using System;
using System.Linq;
using Director.Servicos;
using Keep.Paper.Api;
using Keep.Tools;

namespace Director.Formularios
{
  [Expose]
  [HomePaper]
  public class FormularioDeMonitoramento : BasePaper
  {
    private readonly ServicoDeAuditoria auditoria;

    public FormularioDeMonitoramento(ServicoDeAuditoria auditoria)
    {
      this.auditoria = auditoria;
    }

    public object Index() => new
    {
      Kind = Kind.Paper,

      View = new
      {
        Title = "Monitoramento de Eventos do Sistema",
        Design = Design.Grid
      },

      Embedded = auditoria
        .SelectMany(entrada => entrada.Value.Take(100))
        .OrderByDescending(evento => evento.Data)
        .Take(1000)
        .Select(evento => new
        {
          Data = evento
        }).ToArray(),

      Links = new
      {
        Self = Href.To(HttpContext, GetType(), nameof(Index))
      }
    };
  }
}
