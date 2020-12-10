using System;
using System.Collections.Generic;

namespace Keep.Paper.Api
{
  public interface IAuthCatalog
  {
    ICollection<Type> FindAuthTypes(string domain);
  }
}
