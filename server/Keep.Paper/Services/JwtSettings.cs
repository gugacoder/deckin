using System;
using System.Security.Cryptography;

namespace Keep.Paper.Services
{
  internal class JwtSettings : IJwtSettings
  {
    public byte[] Key { get; private set; }
    public string KeyText => Convert.ToBase64String(Key);

    public byte[] SecurityKey { get; private set; }

    public JwtSettings() => RenewSecurityKey();

    public void RenewSecurityKey() => SecurityKey = new HMACSHA256().Key;
  }
}
