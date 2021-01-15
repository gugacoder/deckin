using System;
using Keep.Tools.Collections;

namespace Keep.Paper.DesignX.Modeling
{
  public class View
  {
    public Resource Resource { get; set; }

    public string Title { get; set; }

    public string Target { get; set; }

    public Data Data { get; set; }

    public Disposition Disposition { get; set; }

    public FieldCollection Fields { get; set; }

    public LinkCollection Links { get; set; }
  }
}