using System;
using Keep.Tools;
using Newtonsoft.Json;

namespace Keep.Paper.Api.Types
{
  public class View : Data
  {
    public View()
    {
    }

    public View(string typeName)
    {
      DataExtensions.SetType(this, typeName);
    }

    protected override string SanitizeTypeName(string typeName)
      => typeName?.Replace(nameof(View), "").ToCamelCase();

    protected virtual string BaseName { get; set; }
    protected virtual string BaseTitle { get; set; }
    protected virtual Link BaseTarget { get; set; }
    protected virtual string BaseExtent { get; set; }

    [JsonProperty(Order = -1110)]
    public string Name { get => BaseName; set => BaseName = value; }

    [JsonProperty(Order = -1100)]
    public string Title { get => BaseTitle; set => BaseTitle = value; }

    [JsonProperty(Order = -1080)]
    public virtual Link Target { get => BaseTarget; set => BaseTarget = value; }

    [JsonProperty(Order = -1075)]
    public virtual string Extent { get => BaseExtent; set => BaseExtent = value; }
  }
}
