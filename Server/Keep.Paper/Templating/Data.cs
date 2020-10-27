using System;
using System.Xml.Serialization;

namespace Keep.Paper.Templating
{
  [XmlInclude(typeof(CsvData))]
  public class Data : Node
  {
    public virtual string Name { get; set; }
    public virtual string Type { get; set; }
  }
}
