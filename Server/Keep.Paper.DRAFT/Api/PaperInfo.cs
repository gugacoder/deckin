using System;
using System.Reflection;

namespace Keep.Hosting.Api
{
  public class PaperInfo
  {
    public Route Route { get; set; }
    public IPaper Paper { get; set; }
    public MethodInfo Method { get; set; }
    public object[] Parameters { get; set; }
  }
}
