using System;
using System.Collections.Generic;
using System.Text;

namespace Keep.Tools
{
  public interface IChangeTypeSupport
  {
    bool ChangeTo(Type targetType, out object target);
  }
}
