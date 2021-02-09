using System;
namespace Keep.Paper.Design
{
  public class Fault
  {
    public int? Code { get; set; }

    public string[] Messages { get; set; }

    public string Field { get; set; }
  }
}
