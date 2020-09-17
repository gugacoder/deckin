using System;
namespace Keep.Paper.Api
{
  [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
  public class FallbackAttribute : Attribute
  {
  }
}
