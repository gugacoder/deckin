using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Keep.Paper.Templating;
using Keep.Tools.Collections;

namespace Keep.Paper.Templating
{
  public class NodeCollection<T> : Collection<T>, INode
    where T : INode
  {
    [JsonIgnore]
    public Template Template => Root as Template;

    [JsonIgnore]
    public INode Root
    {
      get
      {
        var parent = this.Parent;
        while (parent.Parent != null)
        {
          parent = parent.Parent;
        }
        return parent;
      }
    }

    [JsonIgnore]
    public INode Parent { get; set; }

    protected override void OnCommitAdd(ItemStore store, IEnumerable<T> items, int index = -1)
    {
      items.ForEach(x => x.Parent = this);
      base.OnCommitAdd(store, items, index);
    }

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
