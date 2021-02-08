using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Keep.Paper.Design.Rendering
{
  public interface IOutput
  {
    AcceptedFormats AcceptedFormats { get; }

    IFormat BodyFormat { get; }

    Stream Body { get; }
  }
}
