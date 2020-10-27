using System;
using Keep.Tools;
using Newtonsoft.Json;

namespace Keep.Paper.Api.Types
{
  [Serializable]
  public class TextWidget : Widget
  {
    private bool? _password;

    [JsonProperty(Order = 20)]
    public bool? Password
    {
      get
      {
        if (_password != null) return _password;
        if (AutoComplete?.ContainsAnyIgnoreCase("Password") == true) return true;
        return null;
      }
      set => _password = value;
    }

    [JsonProperty(Order = 30)]
    public bool? Multiline { get; set; }

    [JsonProperty(Order = 500)]
    public bool? AllowMany { get; set; }
  }
}
