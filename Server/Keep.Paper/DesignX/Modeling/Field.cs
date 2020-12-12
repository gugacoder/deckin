using System;

namespace Keep.Paper.DesignX.Modeling
{
  public class Field
  {
    public string Name { get; set; }

    public string Title { get; set; }

    public FieldType Type { get; set; }

    public LinkCollection Links { get; set; }
  }
}
