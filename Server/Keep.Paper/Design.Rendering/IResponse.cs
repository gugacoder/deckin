using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Keep.Paper.Design.Rendering
{
  public interface IResponse
  {
    Accept Accept { get; }

    Mime Mime { get; set; }

    Stream Body { get; }
  }
}
