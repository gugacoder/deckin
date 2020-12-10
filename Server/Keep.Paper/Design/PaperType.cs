using System;
using System.Linq;
using System.Runtime.CompilerServices;
using Keep.Tools;
using Microsoft.Extensions.DependencyInjection;

namespace Keep.Paper.Design
{
  public class PaperType
  {
    private string _catalog;
    private string _name;
    private Func<IServiceProvider, IPaper> _factory;

    public PaperType()
    {
    }

    public PaperType(Type type)
    {
      this.Type = type;
    }

    public Type Type
    {
      get;
      set;
    }

    public string Catalog
    {
      get => _catalog ??= Paper.Design.Name.Catalog(Type);
      set => _catalog = value;
    }

    public string Name
    {
      get => _name ??= Paper.Design.Name.Paper(Type);
      set => _name = value;
    }

    public Func<IServiceProvider, IPaper> Factory
    {
      get => _factory ??= CreateDefaultFactory();
      set => _factory = value;
    }

    private Func<IServiceProvider, IPaper> CreateDefaultFactory()
      => (provider) => (IPaper)ActivatorUtilities.CreateInstance(provider, Type);
  }
}
