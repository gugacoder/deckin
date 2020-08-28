using System;
using System.Security.Cryptography;
using System.Text;

namespace Keep.Paper.Api
{
  public static class Crypto
  {
    public static string CreateValidKey(string key)
    {
      if (key.Length < 24)
      {
        key += new string('=', 24 - key.Length);
      }
      return key;
    }

    public static string Encrypt(string input, string key)
    {
      key = CreateValidKey(key);

      var source = UTF8Encoding.UTF8.GetBytes(input);

      var des = new TripleDESCryptoServiceProvider();
      des.Key = UTF8Encoding.UTF8.GetBytes(key);
      des.Mode = CipherMode.ECB;
      des.Padding = PaddingMode.PKCS7;

      var transform = des.CreateEncryptor();
      var target = transform.TransformFinalBlock(source, 0, source.Length);

      des.Clear();

      return Convert.ToBase64String(target, 0, target.Length);
    }

    public static string Decrypt(string input, string key)
    {
      key = CreateValidKey(key);

      var source = Convert.FromBase64String(input);

      var des = new TripleDESCryptoServiceProvider();
      des.Key = UTF8Encoding.UTF8.GetBytes(key);
      des.Mode = CipherMode.ECB;
      des.Padding = PaddingMode.PKCS7;

      var transform = des.CreateDecryptor();
      var target = transform.TransformFinalBlock(source, 0, source.Length);

      des.Clear();

      return UTF8Encoding.UTF8.GetString(target);
    }
  }
}
