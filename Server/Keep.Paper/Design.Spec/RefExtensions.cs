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
  public static class RefExtensions
  {
    public static void CopyTo(this IRef source, IRef target)
      => Copy(source, target);

    public static IRef CastTo(this IRef source, Type targetRefType)
      => Cast(source, targetRefType);

    public static Ref<T> CastTo<T>(this IRef source)
      where T : class, IEntity
      => (Ref<T>)Cast(source, typeof(Ref<T>));

    #region Implementação dos métodos

    public static void Copy(IRef source, IRef target)
    {
      target.BaseType = source.BaseType;
      target.UserType = source.UserType;
      target.Args = new StringMap(source.Args);
    }

    public static IRef Cast(IRef @ref, Type targetRefType)
    {
      if (targetRefType.IsAssignableFrom(@ref.GetType()))
        return @ref;

      var targetRef = (IRef)Activator.CreateInstance(targetRefType);
      Copy(@ref, targetRef);
      return targetRef;
    }

    #endregion
  }
}
