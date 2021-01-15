using System;
namespace Keep.Paper.Design.Rendering
{
  public class Accept
  {
    public string UserAgent { get; set; }
    public Choice[] MimeType { get; set; }
    public Choice[] Charset { get; set; }
    public Choice[] Encoding { get; set; }
    public Choice[] Language { get; set; }
  }
}
