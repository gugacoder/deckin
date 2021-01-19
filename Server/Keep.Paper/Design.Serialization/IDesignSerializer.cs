using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Keep.Paper.Design.Serialization
{
  public interface IDesignSerializer
  {
    Task SerializeAsync<T>(Stream output, T design,
      CancellationToken stopToken = default)
        where T : IDesign;

    Task<T> DeserializeAsync<T>(Stream input,
      CancellationToken stopToken = default)
        where T : IDesign;
  }
}
