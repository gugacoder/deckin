using System;
using System.Collections.Generic;

namespace Keep.Hosting.Auth
{
  public interface IAuthCatalog
  {
    ICollection<Type> FindAuthTypes(string domain);
  }
}
