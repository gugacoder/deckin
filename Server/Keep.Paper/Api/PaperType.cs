using System;
using System.Linq;
using System.Runtime.CompilerServices;
using Keep.Tools;
using Microsoft.Extensions.DependencyInjection;

namespace Keep.Paper.Api
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
      get => _catalog ??= Api.Name.Catalog(Type);
      set => _catalog = value;
    }

    public string Name
    {
      get => _name ??= Api.Name.Paper(Type);
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
