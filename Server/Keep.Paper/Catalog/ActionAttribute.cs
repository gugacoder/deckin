using System;
namespace Keep.Paper.Catalog
{
  [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
  public class ActionAttribute : Attribute
  {
    public ActionAttribute(string pattern)
    {
      this.Pattern = pattern;
    }

    public string Pattern { get; private set; }
  }
}
