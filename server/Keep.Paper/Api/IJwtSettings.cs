using System;
namespace Keep.Paper.Api
{
  public interface IJwtSettings
  {
    byte[] SecurityKey { get; }

    void RenewSecurityKey();
  }
}
