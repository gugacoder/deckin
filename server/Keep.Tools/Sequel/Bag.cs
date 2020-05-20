using Keep.Tools.Collections;
using System;
using System.Collections.Generic;
using System.Text;

namespace Keep.Tools.Sequel
{
  public class Bag : ISettableContract
  {
    internal HashMap Parameters { get; set; }

    #region Implementação de ISettable

    HashMap ISettableContract.GetParameters()
      => Parameters;

    HashMap ISettableContract.EnsureParameters()
      => Parameters ??= new HashMap();

    object ISettableContract.GetParameter(string key)
      => Parameters?[key];

    void ISettableContract.SetParameter(string key, object value)
      => (Parameters ??= new HashMap()).Add(key, value);

    #endregion

  }
}
