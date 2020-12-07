using System;
using System.Text.RegularExpressions;

namespace Keep.Tools
{
  public static class Guids
  {
    public static Guid? ExtractGuidFromString(string @string)
    {
      var regex = new Regex(@"(\w{8}-?\w{4}-?\w{4}-?\w{4}-?\w{12})");
      var match = regex.Match(@string);
      if (match.Success)
      {
        var text = match.Groups[1].Value;
        if (Guid.TryParse(text, out Guid guid))
          return guid;
      }
      return null;
    }

    public static Ret<Guid> TryExtractGuidFromString(string @string)
    {
      try
      {
        var regex = new Regex(@"(\w{8}-?\w{4}-?\w{4}-?\w{4}-?\w{12})");
        var match = regex.Match(@string);
        if (!match.Success)
          throw new ParseException("O texto não contém uma representação válida de GUID.");

        var text = match.Groups[1].Value;
        if (!Guid.TryParse(text, out Guid guid))
          throw new ParseException("O texto não representa um GUID válido: " + text);

        return guid;
      }
      catch (Exception ex)
      {
        return ex;
      }
    }
  }
}
