using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Keep.Paper.Design.Serialization
{
  public static class DesignSerializerExtensions
  {
    public static async Task<string> SerializeAsync(
      this IDesignSerializer serializer, IDesign @object,
      CancellationToken stopToken = default)
    {
      using var memory = new MemoryStream();
      await serializer.SerializeAsync(memory, @object, stopToken);
      memory.Position = 0;
      using var reader = new StreamReader(memory);
      var image = await reader.ReadToEndAsync();
      return image;
    }

    public static async Task<T> DeserializeAsync<T>(
      this IDesignSerializer serializer, string image,
      CancellationToken stopToken = default)
      where T : IDesign
    {
      using var memory = new MemoryStream();
      using var writer = new StreamWriter(memory);
      await writer.WriteAsync(image);
      memory.Position = 0;
      var design = await serializer.DeserializeAsync<T>(memory, stopToken);
      return design;
    }
  }
}