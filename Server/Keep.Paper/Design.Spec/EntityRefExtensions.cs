using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Keep.Paper.Design.Modeling;
using Keep.Tools;
using Keep.Tools.Collections;
using Keep.Tools.Reflection;
using Newtonsoft.Json;

namespace Keep.Paper.Design.Spec
{
  public static class EntityRefExtensions
  {
    public static void CopyEntityRefTo(this IEntityRef source, IEntityRef target)
      => CopyEntityRef(source, target);

    public static IEntityRef CastEntityRefTo(this IEntityRef source, Type targetRefType)
      => CastEntityRef(source, targetRefType);

    public static EntityRef<T> CastEntityRefTo<T>(this IEntityRef source)
      where T : class, IEntity
      => (EntityRef<T>)CastEntityRef(source, typeof(EntityRef<T>));

    #region Implementação dos métodos

    private static void CopyEntityRef(IEntityRef source, IEntityRef target)
    {
      target.Ref = new Ref
      {
        Path = source.Ref?.Path,
        Args = source.Ref?.Args
      };
    }

    private static IEntityRef CastEntityRef(IEntityRef @ref, Type targetRefType)
    {
      if (targetRefType.IsInterface && targetRefType.IsGenericType)
      {
        if (targetRefType == typeof(IEntityRef))
        {
          targetRefType = typeof(EntityRef);
        }
        else // if (targetRefType.GetGenericTypeDefinition() == typeof(IEntityRef<>))
        {
          var entityType = TypeOf.Generic(targetRefType);
          targetRefType = typeof(EntityRef<>).MakeGenericType(entityType);
        }
      }

      var targetRef = (IEntityRef)Activator.CreateInstance(targetRefType);
      CopyEntityRef(@ref, targetRef);
      return targetRef;
    }

    #endregion
  }
}
