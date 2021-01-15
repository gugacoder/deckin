using System;
using System.Collections;
using System.Collections.Generic;
using Keep.Tools.Collections;

namespace Keep.Paper.Design.Modeling
{
  public interface IPayloadChunk
  {
    IEnumerable<IPayload> Chunk { get; set; }
  }
}
