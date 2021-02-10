using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Keep.Tools;
using Keep.Tools.Collections;

namespace Keep.Paper.Design.Spec
{
  public static class EntityExtensions
  {
    public static IEnumerable<IEntity> Children(this IEntity entity)
    {
      var children =
        from property in entity.GetType().GetProperties()
        let propType = property.PropertyType
        let itemType = TypeOf.CollectionElement(property.PropertyType)
        where typeof(IRef).IsAssignableFrom(propType)
           || typeof(IRef).IsAssignableFrom(itemType)
        let value = property.GetValue(entity)
        let entities =
          value is ICollection collection
            ? collection.OfType<IEntity>()
            : (value as IEntity).AsSingle()
        select entities;
      return children.SelectMany().NotNull();
    }

    public static IEnumerable<IEntity> Descendants(this IEntity entity)
    {
      if (entity == null)
        yield break;

      foreach (var child in entity.Children())
      {
        yield return child;
        foreach (var item in Descendants(child))
          yield return item;
      }
    }

    public static IEnumerable<IEntity> DescendantsAndSelf(this IEntity entity)
    {
      if (entity == null)
        yield break;

      yield return entity;
      foreach (var child in Descendants(entity))
        yield return child;
    }
  }
}
