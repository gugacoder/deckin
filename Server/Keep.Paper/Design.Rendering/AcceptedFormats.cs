using System;
namespace Keep.Paper.Design.Rendering
{
  public class AcceptedFormats
  {
    public string UserAgent { get; set; }
    public ChoiceCollection MimeType { get; set; }
    public ChoiceCollection Charset { get; set; }
    public ChoiceCollection Encoding { get; set; }
    public ChoiceCollection Language { get; set; }
  }
}
