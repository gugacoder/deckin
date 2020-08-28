using System;
namespace Keep.Paper.Api
{
  public interface IAuthCatalog
  {
    Type FindAuthType(string domain);
  }
}
