using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Keep.Tools.Collections;

namespace Keep.Paper.Api.Types
{
  public class Payload<TForm> : IPayload, IPayloadChunk
  {
    public TForm Form { get; set; }

    public Collection Data { get; set; }

    public Collection<Payload<TForm>> Chunk
    {
      get => throw new NotImplementedException();
      set => throw new NotImplementedException();
    }

    object IPayload.Form
    {
      get => Form;
      set => Form = (TForm)value;
    }

    IEnumerable IPayload.Data
    {
      get => Data;
      set => Data = (value as Collection) ?? new Collection(value);
    }

    IEnumerable<IPayload> IPayloadChunk.Chunk
    {
      get => Chunk;
      set => Chunk = (value as Collection<Payload<TForm>>)
        ?? new Collection<Payload<TForm>>(value.Cast<Payload<TForm>>());
    }
  }
}
