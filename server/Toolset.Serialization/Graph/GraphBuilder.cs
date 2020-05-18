using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Toolset.Collections;
using Toolset.Reflection;

namespace Toolset.Serialization.Graph
{
  public class GraphBuilder
  {
    public T CreateGraph<T>(NodeModel document)
      where T : new()
    {
      return (T)CreateGraph(document, typeof(T));
    }

    public object CreateGraph(NodeModel document, Type graphType)
    {
      if (document.IsDocument)
      {
        document = ((DocumentModel)document).Root;
      }

      if (typeof(IGraphDeserializer).IsAssignableFrom(graphType))
      {
        var graph = (IGraphDeserializer)Activator.CreateInstance(graphType);
        graph.Deserialize(document, this);
        return graph;
      }

      if (document.IsObject)
      {
        var graph = Activator.CreateInstance(graphType);
        foreach (var property in document.ChildProperties())
        {
          SetProperty(graph, property.Name, property.Value);
        }
        return graph;
      }

      if (document.IsCollection)
      {
        var list = new ArrayList();
        var itemType = TypeOf.CollectionElement(graphType);
        foreach (var child in document.Children())
        {
          var item = CreateGraph(child, itemType);
          list.Add(item);
        }
        var graph = (ICollection)Change.To(list, graphType);
        return graph;
      }

      return document.SerializationValue;
    }

    private void SetProperty(object graph, string property, NodeModel document)
    {
      object value = null;
      bool customValue = false;

      var propertyInfo = graph._GetPropertyInfo(property);
      property = propertyInfo?.Name ?? property.ChangeCase(TextCase.PascalCase);

      if (graph is IPropertyValueSetter setter)
      {
        var done = setter.SetProperty(property, document, this);
        if (done)
          return;
      }

      if (graph is IPropertyValueFactory factory)
      {
        customValue = factory.CreateValue(property, document, this, out value);
        if (!customValue && value != null)
        {
          value = null;
        }
      }

      if (Is.Dictionary(graph))
      {
        var keyType = TypeOf.DictionaryKey(graph);
        var key = Change.To(property, keyType);

        if (!customValue)
        {
          var valueType = TypeOf.DictionaryValue(graph);
          value = CreateGraph(document, valueType);
        }

        var dictionary = (IDictionary)graph;
        dictionary.Add(key, value);

        return;
      }

      if (propertyInfo == null)
        throw new NoSuchPropertyException(graph.GetType(), property);

      if (!customValue)
      {
        value = CreateGraph(document, propertyInfo.PropertyType);
      }
      graph._Set(property, value);
    }

    //private void SetProperty(object graph, string property, object value)
    //{
    //  if (Is.Dictionary(graph))
    //  {
    //    var keyType = TypeOf.DictionaryKey(graph);
    //    var valType = TypeOf.DictionaryValue(graph);

    //    var key = Change.To(property, keyType);
    //    var val = Change.To(property, valType);

    //    var dictionary = (IDictionary)graph;
    //    dictionary.Add(key, val);

    //    return;
    //  }


    //}
  }
}
