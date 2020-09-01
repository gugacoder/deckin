using System;
using System.Xml.Serialization;

namespace Keep.Paper.Api.Types
{
  public abstract class Design
  {
    public abstract string Kind { get; set; }
  }
}
