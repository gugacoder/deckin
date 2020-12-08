using System;
using System.Collections;
using System.Collections.Generic;
using Keep.Tools.Collections;

namespace Keep.Hosting.Api.Types
{
  public interface IPayloadChunk
  {
    IEnumerable<IPayload> Chunk { get; set; }
  }
}
