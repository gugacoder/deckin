using System;
using Keep.Tools.Collections;

namespace Keep.Paper.Catalog
{
  public class CustomCatalog : ICatalog
  {
    private readonly HashMap<IAction> catalog;

    public CustomCatalog()
    {
      this.catalog = new HashMap<IAction>();
    }

    public IAction GetAction(string pathName)
    {
      return catalog[pathName];
    }

    public void Add(IAction action)
    {
      this.catalog[action.Ref.Name] = action;
    }
  }
}
