using System;
using System.Diagnostics;
using Keep.Tools;
using Keep.Tools.Collections;
using System.Linq;
using Keep.Paper.Design;
using Keep.Paper.Design.Serialization;
using Newtonsoft.Json.Linq;
using Keep.Paper.Design.Rendering;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using Keep.Tools.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Mercadologic.Replicacao
{
  public class LocalContext : IDesignContext
  {
  }

  public class LocalFormat : IFormat
  {
    public string MimeType { get; set; }
    public string Charset { get; set; }
    public string Compression { get; set; }
    public string Language { get; set; }
  }

  public class LocalResponse : IResponse, IDisposable
  {
    private readonly MemoryStream memory;

    public LocalResponse()
    {
      memory = new MemoryStream();
    }

    public AcceptedFormats AcceptedFormats { get; } = new AcceptedFormats();

    public IFormat Format { get; } = new LocalFormat();

    public Stream Body => memory;

    public void Dispose() => memory.Dispose();

    public override string ToString()
    {
      memory.Position = 0;
      var reader = new StreamReader(memory);
      var writer = new StringWriter();
      reader.CopyTo(writer);
      return writer.ToString();
    }
  }

  public class Program
  {
    public static async Task DoAsync(string[] args)
    {
      var source = new Data
      {
        Self = Ref.Create("Demo", "Sandbox", new { Id = 10 }),
        Properties = new
        {
          Id = 10,
          Name = "Tenth",
          Date = DateTime.Now
        }
      };

      var memory = new MemoryStream();
      using var writer = new StreamWriter(memory);
      using var jsonWriter = new JsonTextWriter(writer);

      var jObject = JObject.FromObject(source);
      await jObject.WriteToAsync(jsonWriter);

      await jsonWriter.FlushAsync();
      await writer.FlushAsync();



      memory.Position = 0;
      var image = await new StreamReader(memory).ReadToEndAsync();
      Debug.WriteLine("- - -");
      Debug.WriteLine(Json.Beautify(image));
      Debug.WriteLine("- - -");



      memory.Position = 0;
      using var reader = new StreamReader(memory);
      using var jsonReader = new JsonTextReader(reader);

      jObject = await JObject.LoadAsync(jsonReader);
      var target = jObject.ToObject<Data>();

      Debug.WriteLine(Json.Beautify(Json.ToJson(target)));
      Debug.WriteLine("- - -");
    }

    public static async Task Main(string[] args) { try { await DoAsync(args); } catch (Exception ex) { Debug.WriteLine("FAIL!!!"); Debug.WriteLine(ex.GetCauseMessage()); Debug.WriteLine(ex.GetStackTrace()); } }
  }
}