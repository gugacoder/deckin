using System;
using System.Collections;
using System.Collections.Generic;
using Keep.Tools.Collections;

namespace Keep.Paper.Api.Types
{
  public interface IPayloadChunk
  {
    IEnumerable<IPayload> Chunk { get; set; }
  }
}
