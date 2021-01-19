using System;
using System.IO;
using Keep.Paper.Design.Rendering;

namespace Keep.Paper.Design.Core
{
  public class DefaultResponse : IResponse
  {
    public AcceptedFormats AcceptedFormats { get; set; }

    public Format Format { get; set; }

    public Stream Body { get; set; }
  }
}
