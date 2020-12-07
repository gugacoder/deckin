using System;
using System.Security.Cryptography;
using System.Text;

namespace Keep.Hosting.Utilities
{
  public static class Crypto
  {
    public const string Key = "DA51F418-CF10-4455-B500-0FB73004AAE3";

    public static string Encrypt(string input)
    {
      if (input.StartsWith("enc:"))
      {
        return input;
      }
      else
      {
        var hash = Tools.Crypto.Encrypt(input, Key);
        return $"enc:{hash}";
      }
    }

    public static string Decrypt(string input)
    {
      if (input.StartsWith("enc:"))
      {
        input = input.Substring("enc:".Length);
        return Tools.Crypto.Decrypt(input, Key);
      }
      else
      {
        return input;
      }
    }
  }
}
