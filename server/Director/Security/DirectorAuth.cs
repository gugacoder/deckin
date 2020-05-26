using System;
using System.Threading.Tasks;
using Director.Connectors;
using Director.Models;
using Keep.Paper.Api;
using Keep.Paper.Domain;
using Keep.Tools;
using Microsoft.Extensions.DependencyInjection;

namespace Director.Security
{
  [Expose]
  public class DirectorAuth : IAuth
  {
    private readonly IServiceProvider serviceProvider;

    public DirectorAuth(IServiceProvider serviceProvider)
    {
      this.serviceProvider = serviceProvider;
    }

    public async Task<Identity> AuthenticateAsync(Credential credential)
    {
      var model = ActivatorUtilities.CreateInstance<LoginModel>(serviceProvider);
      var identity = await model.AutenticarAsync(credential);
      return identity;
    }
  }
}
