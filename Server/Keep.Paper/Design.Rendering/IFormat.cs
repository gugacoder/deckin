using System;
namespace Keep.Paper.Design.Rendering
{
  public interface IFormat
  {
    string MimeType { get; set; }
    string Charset { get; set; }
    string Compression { get; set; }
    string Language { get; set; }
  }
}
