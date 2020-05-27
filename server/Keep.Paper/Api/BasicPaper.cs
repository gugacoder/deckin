using System;
using System.Runtime.CompilerServices;
using Keep.Paper.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Keep.Paper.Api
{
  public abstract class BasicPaper : IPaper
  {
    protected HttpContext HttpContext { get; private set; }
    protected IServiceProvider ServiceProvider { get; private set; }
    protected IPaperCatalog PaperCatalog { get; private set; }

    protected T CreateInstance<T>(params string[] parameters)
        => ActivatorUtilities.CreateInstance<T>(ServiceProvider, parameters);

    protected object CreateInstance(Type instanceType, params string[] parameters)
        => ActivatorUtilities.CreateInstance(ServiceProvider, instanceType, parameters);

    internal void Configure(HttpContext httpContext, IServiceProvider serviceProvider)
    {
      this.HttpContext = httpContext;
      this.ServiceProvider = serviceProvider;
      this.PaperCatalog = serviceProvider.GetRequiredService<IPaperCatalog>();
    }
  }
}
