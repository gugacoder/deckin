using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Keep.Tools.Collections;

namespace Keep.Paper.Design.Modeling
{
  public class Payload<TForm, TData> : IPayload, IPayloadChunk
  {
    public TForm Form { get; set; }

    public Collection<TData> Data { get; set; }

    public Collection<Payload<TForm, TData>> Chunk { get; set; }

    object IPayload.Form
    {
      get => Form;
      set => Form = (TForm)value;
    }

    IEnumerable IPayload.Data
    {
      get => Data;
      set => Data = (value as Collection<TData>)
        ?? new Collection<TData>(value.Cast<TData>());
    }

    IEnumerable<IPayload> IPayloadChunk.Chunk
    {
      get => Chunk;
      set => Chunk = (value as Collection<Payload<TForm, TData>>)
        ?? new Collection<Payload<TForm, TData>>(value.Cast<Payload<TForm, TData>>());
    }
  }
}
