using System;
using System.Threading.Tasks;
using AppSuite.Modelos;
using Keep.Paper.Api;
using Keep.Paper.Types;
using Keep.Tools;
using Microsoft.Extensions.DependencyInjection;

namespace AppSuite.Adaptadores
{
  [Expose]
  public class DirectorAuth : IAuth
  {
    private readonly IServiceProvider serviceProvider;

    public DirectorAuth(IServiceProvider serviceProvider)
    {
      this.serviceProvider = serviceProvider;
    }

    public async Task<Ret<UserIdentity>> AuthenticateAsync(Credential credential)
    {
      try
      {
        var model = ActivatorUtilities.CreateInstance<Login>(serviceProvider);
        var ret = await model.AutenticarAsync(credential);
        return ret;
      }
      catch (Exception ex)
      {
        return ex;
      }
    }
  }
}
