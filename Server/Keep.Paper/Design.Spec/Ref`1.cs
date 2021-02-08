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
  [JsonConverter(typeof(RefConverter))]
  public class Ref<T> : Ref, IRef<T>
    where T : class, IEntity
  {
    #region Conversões implícitas

    public static implicit operator string(Ref<T> @ref) => @ref?.ToString();

    public static implicit operator Ref<T>(string @ref) => Ref.Parse<T>(@ref);

    #endregion
  }
}