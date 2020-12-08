using System;
namespace Keep.Hosting.Api
{
  [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface,
      AllowMultiple = false, Inherited = true)]
  public class HomePaperAttribute : Attribute
  {
  }
}
