using System;
using Keep.Tools.Collections;

namespace Keep.Paper.Design
{
  public class Paper : Entity
  {
    public Collection<Data> Data { get; set; }

    public View View { get; set; }
  }
}
