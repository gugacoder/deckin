using System;
namespace Keep.Paper.Services
{
  public interface IAuthTypeCollection
  {
    Type FindAuthType(string domain);
  }
}
