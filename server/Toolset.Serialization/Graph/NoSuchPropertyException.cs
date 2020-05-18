using System;
using System.Collections.Generic;
using System.Text;

namespace Toolset.Serialization.Graph
{
  public class NoSuchPropertyException : SerializationException
  {
    public NoSuchPropertyException(Type host, string property)
      : base($"A propriedade não existe: {host.FullName}.{property}")
    {
    }
  }
}
