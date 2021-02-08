using System;
using System.Collections.Generic;
using System.Linq;
using Keep.Tools.Collections;
using Keep.Paper.Design.Spec;

namespace Keep.Paper.Design.Rendering
{
  public static class ResponseExtensions
  {
    public static void Normalize(this Response response)
    {
      var entities = DescendantsAndSelf(response.Data);
      entities.ForEach(entity => entity.Self ??= Ref.ForLocalReference(entity));
      response.Embedded ??= new Collection<IEntity>();
      response.Embedded.AddMany(entities.Except(response.Embedded));
    }

    private static IEnumerable<IEntity> DescendantsAndSelf(IEntity entity)
    {
      yield return entity;
      foreach (var child in entity.Children())
        foreach (var item in DescendantsAndSelf(child))
          yield return item;
    }
  }
}
