using System;
using Keep.Tools.Collections;

namespace Keep.Paper.DesignX
{
  public class Form
  {
    public string Name { get; set; }

    public string Title { get; set; }

    public DataSet Data { get; set; }

    public View View { get; set; }

    public FieldCollection Fields { get; set; }

    public LinkCollection Links { get; set; }
  }
}
