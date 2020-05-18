using System;
using System.Collections.Generic;
using System.Text;

namespace Toolset
{
  [AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = true)]
  public class IgnoreAttribute : Attribute
  {
  }
}
