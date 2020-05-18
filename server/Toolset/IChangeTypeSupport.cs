using System;
using System.Collections.Generic;
using System.Text;

namespace Toolset
{
  public interface IChangeTypeSupport
  {
    bool ChangeTo(Type targetType, out object target);
  }
}
