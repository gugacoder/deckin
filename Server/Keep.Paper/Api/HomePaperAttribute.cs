using System;
namespace Keep.Paper.Api
{
  [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface,
      AllowMultiple = false, Inherited = true)]
  public class HomePaperAttribute : Attribute
  {
  }
}
