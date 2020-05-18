using System;
using System.Collections.Generic;
using System.Text;

namespace Toolset.Serialization.Graph
{
  public interface IPropertyValueFactory
  {
    bool CreateValue(string property, NodeModel document, GraphBuilder builder, out object value);
  }
}
