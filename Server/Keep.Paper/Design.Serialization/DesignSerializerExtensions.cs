using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Keep.Paper.Design.Serialization
{
  public static class DesignSerializerExtensions
  {
    public static async Task<string> SerializeAsync(
      this IDesignSerializer serializer, IDesign design,
      CancellationToken stopToken = default)
    {
      using var writer = new StringWriter();
      await serializer.SerializeAsync(design, writer, stopToken);
      var image = writer.ToString();
      return image;
    }

    public static async Task<T> DeserializeAsync<T>(
      this IDesignSerializer serializer, string image,
      CancellationToken stopToken = default)
      where T : IDesign
    {
      using var reader = new StringReader(image);
      var design = await serializer.DeserializeAsync<T>(reader, stopToken);
      return design;
    }
  }
}