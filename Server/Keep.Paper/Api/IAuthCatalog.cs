using System;
using System.Collections.Generic;

namespace Keep.Hosting.Api
{
  public interface IAuthCatalog
  {
    ICollection<Type> FindAuthTypes(string domain);
  }
}
