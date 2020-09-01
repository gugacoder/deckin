using System;
namespace Keep.Paper.Api.Types
{
  public abstract class Template
  {
    public string Catalog { get; set; }

    public string Name { get; set; }

    public string Title { get; set; }

    public bool? Enabled { get; set; }

    public TDesign Design { get; set; }

    public class TDesign
    {
      public GridDesign Grid { get; set; }
    }
  }
}
