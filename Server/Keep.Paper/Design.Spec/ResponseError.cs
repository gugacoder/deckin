using System;
namespace Keep.Paper.Design.Spec
{
  public class ResponseError
  {
    public int? Code { get; set; }
    public string[] Messages { get; set; }
  }
}
