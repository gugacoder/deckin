using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using Keep.Paper.Api;
using Keep.Tools;
using Keep.Tools.Collections;
using static Director.Servicos.ServicoDeAuditoria;

namespace Director.Servicos
{
  public class ServicoDeAuditoria : HashMap<Eventos>
  {
    private const int MaxRecordPerEvent = 1000;

    public void Add(Evento evento)
    {
      var key = $"{evento.Origem} {evento.Nome}";
      var deque = (this[key] ??= new Eventos());
      deque.Add(evento);
    }

    public class Evento
    {
      private long idGenerator;

      public Evento()
      {
        this.Id = ++idGenerator;
      }

      public long Id { get; }
      public Level Nivel { get; set; }
      public string Origem { get; set; }
      public string Nome { get; set; }
      public DateTime Data { get; set; }
      public string Mensagem { get; set; }
    }

    public class Eventos : Collection<Evento>
    {
      protected override void OnCommitAdd(ItemStore store, IEnumerable<Evento> items, int index = -1)
      {
        while (Count >= MaxRecordPerEvent)
        {
          base.RemoveFirst();
        }
        base.OnCommitAdd(store, items, index);
      }
    }
  }
}