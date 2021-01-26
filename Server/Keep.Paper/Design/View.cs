using System;
using Keep.Tools.Collections;

namespace Keep.Paper.Design
{
  public class View : Entity
  {
    public string Target { get; set; }

    public PaperFace Face { get; set; }

    public Collection<Field> Fields { get; set; }
  }
}
