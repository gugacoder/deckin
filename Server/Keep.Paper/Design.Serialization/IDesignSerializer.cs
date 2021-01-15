using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Keep.Paper.Design.Serialization
{
  public interface IDesignSerializer
  {
    Task SerializeAsync(IDesign design, TextWriter writer,
      CancellationToken stopToken = default);

    Task<T> DeserializeAsync<T>(TextReader reader,
      CancellationToken stopToken = default)
      where T : IDesign;
  }
}
