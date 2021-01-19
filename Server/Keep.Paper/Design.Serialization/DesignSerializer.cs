using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Keep.Paper.Design.Serialization
{
  public class DesignSerializer : IDesignSerializer
  {
    public async Task SerializeAsync<T>(Stream output, T @object,
      CancellationToken stopToken = default)
        where T : IDesign
    {
      using var writer = new StreamWriter(output, encoding: Encoding.UTF8,
        leaveOpen: true);
      using var jsonWriter = new JsonTextWriter(writer);

      var jObject = JObject.FromObject(@object);
      await jObject.WriteToAsync(jsonWriter, stopToken);

      await jsonWriter.FlushAsync();
      await writer.FlushAsync();
    }

    public async Task<T> DeserializeAsync<T>(Stream input,
      CancellationToken stopToken = default)
        where T : IDesign
    {
      using var reader = new StreamReader(input, Encoding.UTF8);
      using var jsonReader = new JsonTextReader(reader);

      var jObject = await JObject.LoadAsync(jsonReader, stopToken);
      var target = jObject.ToObject<T>();

      return target;
    }
  }
}
