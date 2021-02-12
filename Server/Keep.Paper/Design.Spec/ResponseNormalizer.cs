using System;
using System.Collections.Generic;
using System.Linq;
using Keep.Tools;
using Keep.Tools.Collections;
using Keep.Tools.Reflection;

namespace Keep.Paper.Design.Spec
{
  public class ResponseNormalizer
  {
    public void Normalize(IResponse response)
    {
      var embedded = new HashMap<IEntity>();
      if (response.Embedded != null)
      {
        response.Embedded
          .Where(e => e.Self != null)
          .ForEach(e => embedded.Add(e.Self.ToString(), e)); 
      }

      var entities = embedded.Values
        .Union(response.Entity.DescendantsAndSelf())
        .ToArray();

      foreach (var entity in entities)
      {
        var properties =
          from property in entity.GetType().GetProperties()
          let propType = property.PropertyType
          let itemType = TypeOf.CollectionElement(property.PropertyType)
          where typeof(IEntityRef).IsAssignableFrom(propType)
             || typeof(IEntityRef).IsAssignableFrom(itemType)
          select property;

        foreach (var property in properties)
        {
          var content = property.GetValue(entity);
          if (content is IEntity child)
          {
            if (child.Self != null)
            {
              IEntityRef @ref = child.CastEntityRefTo(property.PropertyType);
              property.SetValue(entity, @ref);
              embedded.Add(child.Self.ToString(), child);
            }
          }
          else if (content is IExtendedCollection collection)
          {
            var elementType = TypeOf.CollectionElement(collection);
            for (int i = 0; i < collection.Count; i++)
            {
              if (collection[i] is IEntity item && item.Self != null)
              {
                IEntityRef @ref = item.CastEntityRefTo(elementType);
                collection[i] = @ref;
                embedded.Add(item.Self.ToString(), item);
              }
            }
          }
        }
      }

      response.Embedded = (embedded.Count > 0)
        ? new Collection<IEntity>(embedded.Values)
        : null;
    }
  }
}
