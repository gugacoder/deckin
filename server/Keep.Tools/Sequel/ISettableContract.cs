using Keep.Tools.Collections;
using System;
using System.Collections.Generic;
using System.Text;

namespace Keep.Tools.Sequel
{
  internal interface ISettableContract : ISettable
  {
    HashMap GetParameters();

    HashMap EnsureParameters();

    object GetParameter(string key);

    void SetParameter(string key, object value);
  }
}
