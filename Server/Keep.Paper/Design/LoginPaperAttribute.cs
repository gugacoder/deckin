using System;
namespace Keep.Paper.Design
{
  [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface,
      AllowMultiple = false, Inherited = true)]
  public class LoginPaperAttribute : Attribute
  {
  }
}
