using System;
namespace Keep.Hosting.Templating
{
  public class CsvData : Data
  {
    private FieldCollection _fields;

    public bool? HasHeaders { get; set; }

    public string Content { get; set; }

    public FieldCollection Fields
    {
      get => _fields;
      set => _fields = Adopt(value);
    }
  }
}
