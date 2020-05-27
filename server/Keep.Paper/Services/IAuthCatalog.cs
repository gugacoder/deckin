using System;
namespace Keep.Paper.Services
{
  public interface IAuthCatalog
  {
    Type FindAuthType(string domain);
  }
}
