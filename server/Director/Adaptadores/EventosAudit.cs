using System;
using System.Linq;
using Director.Servicos;
using Keep.Paper.Api;
using Keep.Tools;
using static Director.Servicos.ServicoDeAuditoria;

namespace Director.Adaptadores
{
  [Expose]
  public class EventosAudit : IAuditListener
  {
    private readonly ServicoDeAuditoria auditoria;

    public EventosAudit(ServicoDeAuditoria auditoria)
    {
      this.auditoria = auditoria;
    }

    public void Audit(Level level, Type source, string @event, string message)
    {
      auditoria.Add(new Evento
      {
        Nivel = level,
        Origem = source.FullName.Split(',', ':').First(),
        Nome = @event,
        Data = DateTime.Now,
        Mensagem = message
      });
    }
  }
}
