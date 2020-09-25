using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Keep.Tools.Collections;

namespace Keep.Paper.Api.Types
{
  public class Payload : IPayload, IPayloadChunk
  {
    public object Form { get; set; }

    public Collection Data { get; set; }

    public Collection<Payload> Chunk { get; set; }

    IEnumerable IPayload.Data
    {
      get => Data;
      set => Data = (value as Collection) ?? new Collection(value);
    }

    IEnumerable<IPayload> IPayloadChunk.Chunk
    {
      get => Chunk;
      set => Chunk = (value as Collection<Payload>)
        ?? new Collection<Payload>(value.Cast<Payload>());
    }
  }
}
