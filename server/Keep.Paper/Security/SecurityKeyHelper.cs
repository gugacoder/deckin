using System;
using System.IO;
using System.Security.Cryptography;
using System.Xml.Linq;
using Keep.Tools;
using Keep.Tools.Xml;
using Microsoft.IdentityModel.Tokens;

namespace Keep.Paper.Security
{
  public static class SecurityKeyHelper
  {
    private static SecurityKey defaultSecurityKey;
    private static readonly object @lock = new object();

    public static SecurityKey CreateSecurityKey(int seed = 2048)
    {
      using var provider = new RSACryptoServiceProvider(seed);
      var securityKey = new RsaSecurityKey(provider.ExportParameters(true));
      return securityKey;
    }

    public static void CreateSecurityKey(out XElement xml, int seed = 2048)
    {
      using var provider = new RSACryptoServiceProvider(seed);
      xml = provider.ToXmlString(true).ToXElement();
    }

    public static void CreateSecurityKey(out string xml, int seed = 2048)
    {
      using var provider = new RSACryptoServiceProvider(seed);
      xml = provider.ToXmlString(true);
    }

    public static SecurityKey RestoreSecurityKey(XElement xml, int seed = 2048)
    {
      return RestoreSecurityKey(xml.ToXmlString(), seed);
    }

    public static SecurityKey RestoreSecurityKey(string xml, int seed = 2048)
    {
      using var provider = new RSACryptoServiceProvider(seed);
      provider.FromXmlString(xml);
      var securityKey = new RsaSecurityKey(provider.ExportParameters(true));
      return securityKey;
    }

    public static SecurityKey RestoreDefaultSecurityKey()
    {
      lock (@lock)
      {
        try
        {
          if (defaultSecurityKey != null)
            return defaultSecurityKey;

          var securityText = LoadSecurityText();
          if (securityText == null)
          {
            CreateSecurityKey(out securityText);
            SaveSecurityText(securityText);
          }
          return defaultSecurityKey = RestoreSecurityKey(securityText);
        }
        catch (Exception ex)
        {
          ex.Trace();
          return defaultSecurityKey = CreateSecurityKey();
        }
      }
    }

    public static SecurityKey ResetDefaultSecurityKey()
    {
      lock (@lock)
      {
        try
        {
          defaultSecurityKey = null;

          CreateSecurityKey(out string securityText);
          SaveSecurityText(securityText);

          return defaultSecurityKey = RestoreSecurityKey(securityText);
        }
        catch (Exception ex)
        {
          ex.Trace();
          return defaultSecurityKey = CreateSecurityKey();
        }
      }
    }

    private static string LoadSecurityText()
    {
      try
      {
        var file = Path.Combine(App.SharedPath, ".meta", "key-info.xml");
        return FileSystem.LoadTextFileIfExists(file);
      }
      catch (Exception ex)
      {
        ex.Trace();
        return null;
      }
    }

    private static void SaveSecurityText(string securityText)
    {
      try
      {
        var file = Path.Combine(App.SharedPath, ".meta", "key-info.xml");
        FileSystem.SaveTextFile(file, securityText);
      }
      catch (Exception ex)
      {
        ex.Trace();
      }
    }
  }
}
