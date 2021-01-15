using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Keep.Tools;
using Newtonsoft.Json;

namespace Keep.Paper.Design.Serialization
{
  public class JsonDesignSerializer : IDesignSerializer
  {
    public async Task SerializeAsync(IDesign design, TextWriter writer,
      CancellationToken stopToken)
    {
      var settings = new JsonSerializerSettings
      {
        DefaultValueHandling = DefaultValueHandling.Ignore,
        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
      };

      using var jsonWriter = new JsonTextWriter(writer);

      var serializer = JsonSerializer.Create(settings);
      serializer.Serialize(jsonWriter, design);

      await jsonWriter.FlushAsync();
      await writer.FlushAsync();
    }

    public async Task<T> DeserializeAsync<T>(TextReader reader,
      CancellationToken stopToken)
      where T : IDesign
    {
      var settings = new JsonSerializerSettings
      {
        DefaultValueHandling = DefaultValueHandling.Ignore,
        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
      };

      using var jsonReader = new JsonTextReader(reader);

      var serializar = JsonSerializer.Create(settings);
      var design = serializar.Deserialize<T>(jsonReader);

      return await Task.FromResult(design);
    }
  }
}
