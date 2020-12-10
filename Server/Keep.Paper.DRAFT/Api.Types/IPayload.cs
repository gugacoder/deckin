using System;
using System.Collections;
using System.Collections.Generic;
using Keep.Tools.Collections;

namespace Keep.Paper.Api.Types
{
  public interface IPayload
  {
    object Form { get; set; }

    IEnumerable Data { get; set; }
  }
}
