using System;
namespace Keep.Hosting.Api
{
  [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
  public class FallbackAttribute : Attribute
  {
  }
}
