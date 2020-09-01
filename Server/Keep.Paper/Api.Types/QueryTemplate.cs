using System;
using Newtonsoft.Json;

namespace Keep.Paper.Api.Types
{
  [Serializable]
  public class QueryTemplate : Template
  {
    public string Connection { get; set; }

    public string Query { get; set; }
  }
}
