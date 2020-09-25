using System;
using Keep.Tools;
using Newtonsoft.Json;

namespace Keep.Paper.Api.Types
{
  public class Widget : Data
  {
    public Widget()
    {
    }

    public Widget(string typeName)
    {
      DataExtensions.SetType(this, typeName);
    }

    protected override string SanitizeTypeName(string typeName)
      => typeName?.Replace(nameof(Widget), "").ToCamelCase();

    protected virtual string BaseName { get; set; }
    protected virtual string BaseTitle { get; set; }
    protected virtual bool? BaseHidden { get; set; }
    protected virtual bool? BaseRequired { get; set; }
    protected virtual string BaseExtent { get; set; }

    [JsonProperty(Order = -1110)]
    public string Name { get => BaseName; set => BaseName = value; }

    [JsonProperty(Order = -1100)]
    public string Title { get => BaseTitle; set => BaseTitle = value; }

    [JsonProperty(Order = -1080)]
    public bool? Hidden { get => BaseHidden; set => BaseHidden = value; }

    [JsonProperty(Order = -1075)]
    public bool? Required { get => BaseRequired; set => BaseRequired = value; }

    [JsonProperty(Order = -1070)]
    public virtual string Extent { get => BaseExtent; set => BaseExtent = value; }
  }

}
