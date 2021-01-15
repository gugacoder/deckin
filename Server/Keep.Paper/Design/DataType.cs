using System;
using Keep.Tools.Collections;

namespace Keep.Paper.Design
{
  public class DataType : Entity
  {
    public string Type { get; set; }

    public Collection<Field> Fields { get; set; }

    public Collection<Reference> References { get; set; }

    public class Field
    {
    }

    public class Reference
    {
      public bool PKey { get; set; }
      public Collection<string> Keys { get; set; }
    }
  }
}
