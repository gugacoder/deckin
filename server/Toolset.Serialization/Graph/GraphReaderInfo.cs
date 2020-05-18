using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Toolset.Serialization.Graph
{
  internal sealed class GraphReaderInfo : IGraphReaderInfo
  {
    private IGraphReaderInfo instancia;

    public GraphReaderInfo(Type type)
    {
      var isDataContract = type.GetCustomAttributes(typeof(DataContractAttribute), true).Any();
      var isXml =
        type.GetCustomAttributes(typeof(XmlTypeAttribute), true).Any()
        || type.GetCustomAttributes(typeof(XmlRootAttribute), true).Any();
      
      if (isDataContract)
        instancia = new DataContractGraphReaderInfo(type);
      else if (isXml)
        instancia = new XmlGraphReaderInfo(type);
      else
        instancia = new ObjectGraphReaderInfo(type);
    }

    public string GetLabel()
    {
      return instancia.GetLabel();
    }

    public string GetPropertyLabel(PropertyInfo property)
    {
      return instancia.GetPropertyLabel(property);
    }

    public IEnumerable<PropertyInfo> GetProperties()
    {
      return instancia.GetProperties();
    }
  }
}
