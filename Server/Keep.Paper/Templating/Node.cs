using System;
using Newtonsoft.Json;

namespace Keep.Hosting.Templating
{
  public abstract class Node : INode
  {
    [JsonIgnore]
    public Template Template => Root as Template;

    [JsonIgnore]
    public INode Root
    {
      get
      {
        INode node = this;
        while (node.Parent != null)
        {
          node = node.Parent;
        }
        return node;
      }
    }

    [JsonIgnore]
    public INode Parent { get; set; }

    protected T Adopt<T>(T child)
      where T : INode
    {
      if (child != null)
      {
        child.Parent = this;
      }
      return child;
    }
  }
}
