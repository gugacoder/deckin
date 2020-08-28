using System;
using System.Linq;
using Keep.Paper.Api;
using Keep.Tools;

namespace Keep.Paper.Services
{
  internal class AuthCatalog : IAuthCatalog
  {
    private static Type[] authenticators;

    public AuthCatalog()
    {
      Initialize();
    }

    private void Initialize()
    {
      if (authenticators == null)
      {
        authenticators = ExposedTypes.GetTypes<IAuth>().ToArray();
      }
    }

    public Type FindAuthType(string domain)
    {
      // FIXME: Como um IAuth poderia ser associado a um dominio?
      return authenticators.FirstOrDefault();
    }
  }
}
