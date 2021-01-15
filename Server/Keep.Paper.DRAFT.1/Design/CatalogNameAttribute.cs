using System;
namespace Keep.Paper.Design
{
  [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Method)]
  public class CatalogNameAttribute : Attribute
  {
    public CatalogNameAttribute()
    {
    }

    public CatalogNameAttribute(string catalogName)
    {
      this.CatalogName = catalogName;
    }

    public string CatalogName { get; set; }
  }
}
