using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Keep.Paper.Design.Rendering
{
  public interface IResponse
  {
    AcceptedFormats AcceptedFormats { get; }

    IFormat Format { get; }

    Stream Body { get; }
  }
}
