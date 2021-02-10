using System;
namespace Keep.Paper.Design.Spec
{
  [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
  public class BaseTypeAttribute : Attribute
  {
    public BaseTypeAttribute()
    {
    }

    public BaseTypeAttribute(string name)
    {
      Name = name;
    }

    public string Name { get; }
  }
}
