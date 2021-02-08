using System;
using System.Text.Json.Serialization;
using Keep.Tools.Collections;

namespace Keep.Paper.Design.Spec
{
  public interface IRef
  {
    string BaseType { get; set; }
    string UserType { get; set; }
    StringMap Args { get; set; }
    string ToString();
  }
}
