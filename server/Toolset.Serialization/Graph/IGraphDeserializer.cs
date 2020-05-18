using System;
using System.Collections.Generic;
using System.Text;

namespace Toolset.Serialization.Graph
{
  public interface IGraphDeserializer
  {
    void Deserialize(NodeModel document, GraphBuilder builder);
  }
}
