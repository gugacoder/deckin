using System;
using System.Text;
using Keep.Hosting.Api;
using Keep.Tools;
using Microsoft.Extensions.Configuration;

namespace Keep.Hosting.Auth
{
  public class SecretKey
  {
    public SecretKey(IConfiguration configuration, IAudit<SecretKey> audit)
    {
      try
      {
        if (configuration?["Host:SecretKey:Hash"] is string hash)
        {
          this.Hash = hash;
        }
        else if (configuration?["Host:SecretKey:Text"] is string text)
        {
          this.Text = text;
        }
      }
      catch (Exception ex)
      {
        audit?.LogDanger(
          To.Text(
            "A chave secreta para codificação de tokens de segurança está " +
            "configurada indevidamente no arquivo de configuração. " +
            "O sistema seguirá utilizando uma chave aleatória gerada por ele.",
            ex));
      }
      finally
      {
        this.Text ??= App.Name;
      }
    }

    /// <summary>
    /// Versão binária da chave secreta.
    /// </summary>
    public byte[] Bits
    {
      get;
      set;
    }

    /// <summary>
    /// Imagem da chave secreta codificada na base 64.
    /// </summary>
    public string Hash
    {
      get => (Bits != null) ? Convert.ToBase64String(Bits) : null;
      set => Bits = (value != null) ? Convert.FromBase64String(value) : null;
    }

    /// <summary>
    /// Versão textual da chave.
    /// </summary>
    public string Text
    {
      get => (Bits != null) ? Encoding.UTF8.GetString(Bits).Trim() : null;
      set => Bits = (value != null)
        ? Encoding.UTF8.GetBytes(value.Trim().PadRight(16, ' ')) : null;
    }

    public static SecretKey CreateDefault()
      => new SecretKey(null, null);

    public static SecretKey CreateDefault(byte[] bits)
      => new SecretKey(null, null) { Bits = bits };

    public override string ToString() => Hash;
  }
}
