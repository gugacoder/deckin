using System;
using System.Collections.Generic;
using System.Text;

namespace Keep.Tools
{
  [AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = true)]
  public class IgnoreAttribute : Attribute
  {
  }
}
