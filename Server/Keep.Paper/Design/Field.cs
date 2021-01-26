using System;
namespace Keep.Paper.Design
{
  public class Field : Entity
  {
    public string Name { get; set; }

    public string Type { get; set; }

    public FieldFace Face { get; set; }

    public string Title { get; set; }

    public object Value { get; set; }

    public string Severity { get; set; }

    public bool? Hidden { get; set; }

    public bool? Password { get; set; }
  }
}
