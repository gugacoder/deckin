using System;
namespace Keep.Paper.Services
{
  public interface IJwtSettings
  {
    byte[] SecurityKey { get; }

    void RenewSecurityKey();
  }
}
