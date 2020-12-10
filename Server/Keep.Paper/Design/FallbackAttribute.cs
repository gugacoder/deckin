using System;
namespace Keep.Paper.Design
{
  [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
  public class FallbackAttribute : Attribute
  {
  }
}
