﻿using System;
using System.Collections.Generic;
using System.Linq;
using Keep.Paper.Api;
using Keep.Paper.Papers;
using Keep.Tools;
using Keep.Tools.Collections;

namespace Keep.Paper.Services
{
  internal class PaperCatalog : IPaperCatalog
  {
    public const string Home = nameof(Home);
    public const string Login = nameof(Login);

    private Type[] paperTypes;
    private HashMap<Type> specialTypes;

    public PaperCatalog()
    {
      this.paperTypes = ExposedTypes.GetTypes<IPaper>().ToArray();
      this.specialTypes = new HashMap<Type>();
    }

    public Type GetType(string specialName) => specialTypes[specialName];
    public void SetType(string specialName, Type paperType) => specialTypes[specialName] = paperType;

    public Type GetType(string catalogName, string paperName)
    {
      var paperType = (
        from type in paperTypes
        where Name.Catalog(type).Equals(catalogName)
           && Name.Paper(type).Equals(paperName)
        select type
      ).FirstOrDefault();
      return paperType;
    }

    public IEnumerable<string> EnumerateCatalogs()
        => paperTypes.Select(Name.Catalog).Distinct().OrderBy();

    public IEnumerable<string> EnumeratePapers(string catalogName)
        => paperTypes.Where(x => Name.Catalog(x).Equals(catalogName))
              .Select(Name.Paper).OrderBy();

    public IEnumerable<Type> EnumerateTypes(string catalogName)
        => paperTypes.Where(x => Name.Catalog(x).Equals(catalogName))
              .OrderBy(Name.Paper);
  }
}
