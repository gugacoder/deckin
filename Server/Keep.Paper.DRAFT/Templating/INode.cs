using System;

namespace Keep.Hosting.Templating
{
  public interface INode
  {
    Template Template { get; }
    INode Root { get; }
    INode Parent { get; set; }
  }
}
