using System;
using System.Linq;
using Keep.Paper.Api;
using Keep.Tools;

namespace Keep.Paper.Services
{
  internal class PaperTypeCollection : IPaperTypeCollection
  {
    private static Type[] paperTypes;

    public PaperTypeCollection()
    {
      Initialize();
    }

    private void Initialize()
    {
      if (paperTypes == null)
      {
        paperTypes = ExposedTypes.GetTypes<IPaper>().ToArray();
      }
    }

    public Type FindPaperType(string catalog, string paper)
    {
      var paperType = (
        from type in paperTypes
        where type.Assembly.GetName().Name.EqualsIgnoreCase(catalog)
           && type.Name.EqualsAnyIgnoreCase(paper, $"{paper}Paper")
        select type
        ).FirstOrDefault();
      return paperType;
    }
  }
}
