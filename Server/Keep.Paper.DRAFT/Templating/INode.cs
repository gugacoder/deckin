using System;

namespace Keep.Paper.Templating
{
  public interface INode
  {
    Template Template { get; }
    INode Root { get; }
    INode Parent { get; set; }
  }
}
