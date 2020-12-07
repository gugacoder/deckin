using System;
using Microsoft.Extensions.DependencyInjection;

namespace Keep.Hosting.Core.Extensions
{
  public static class ServiceProviderExtensions
  {
    public static T Instantiate<T>(this IServiceProvider serviceProvider,
      params object[] parameters)
      where T : class
    {
      return ActivatorUtilities.CreateInstance<T>(serviceProvider, parameters);
    }

    public static object Instantiate(this IServiceProvider serviceProvider,
      Type instanceType, params object[] parameters)
    {
      return ActivatorUtilities.CreateInstance(serviceProvider, instanceType,
        parameters);
    }
  }
}
