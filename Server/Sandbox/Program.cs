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

namespace Mercadologic.Replicacao
{
  public class LocalContext : IDesignContext
  {
  }

  public class LocalResponse : IResponse, IDisposable
  {
    private readonly MemoryStream memory;

    public LocalResponse()
    {
      memory = new MemoryStream();
    }

    public AcceptedFormats AcceptedFormats { get; } = new AcceptedFormats();

    public Format Format { get; set; } = new Format();

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
    public static void Do(string[] args)
    {
      var builder = new ServiceCollection();
      var services = builder.BuildServiceProvider();

      var ctx = new LocalContext();

      var req = new Request();
      req.Target = new Ref("Demo", "SayHi", new { Name = "Guga Coder" });

      var res = new LocalResponse();

      var pipeline = new RenderingPipeline(services);
      var task = pipeline.RenderAsync(ctx, req, res);

      task.Wait();

      var json = res.ToString();
      Debug.WriteLine(Json.Beautify(json));
    }

    public static void Main(string[] args) { try { Do(args); } catch (Exception ex) { Debug.WriteLine("FAIL!!!"); Debug.WriteLine(ex.GetCauseMessage()); Debug.WriteLine(ex.GetStackTrace()); } }
  }
}