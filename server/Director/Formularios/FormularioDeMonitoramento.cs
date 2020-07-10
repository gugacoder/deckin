using System;
using System.Linq;
using Director.Servicos;
using Keep.Paper.Api;
using Keep.Tools;
using Keep.Tools.Collections;

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
        Design = Design.Grid,
        AutoRefresh = 1 // segundos
      },

      Embedded = auditoria
        .SelectMany(item => item.Value)
        .NotNull()
        .OrderByDescending(evento => evento.Data)
        .Take(40)
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
