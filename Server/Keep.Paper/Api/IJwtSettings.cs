using System;
namespace Keep.Hosting.Api
{
  public interface IJwtSettings
  {
    byte[] SecurityKey { get; }

    void RenewSecurityKey();
  }
}
