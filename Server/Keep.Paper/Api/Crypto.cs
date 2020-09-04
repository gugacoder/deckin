using System;
using System.Security.Cryptography;
using System.Text;

namespace Keep.Paper.Api
{
  public static class Crypto
  {
    public const string Key = "Keep.Paper.Api.Crypto";

    public static string Encrypt(string input)
    {
      if (input.StartsWith("enc:"))
      {
        return input;
      }
      else
      {
        var hash = Tools.Crypto.Decrypt(input, Key);
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
