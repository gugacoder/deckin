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

namespace Mercadologic.Replicacao
{
  public class LocalContext : IDesignContext
  {
  }

  public class LocalResponse : IResponse, IDisposable
  {
    private MemoryStream memory;

    public LocalResponse()
    {
      memory = new MemoryStream();
    }

    public Accept Accept { get; } = new Accept();

    public Mime Mime { get; set; }

    public Stream Body => memory;

    public void Dispose() => memory.Dispose();

    public override string ToString()
    {
      memory.Position = 0;
      var reader = new StreamReader(memory);
      return reader.ToString();
    }
  }

  public class Program
  {
    public static void Do(string[] args)
    {
      var request = new Request
      {
        Target = "Demo/Echo(Message=Hi you there!)"
      };

      var builder = new ServiceCollection();
      var services = builder.BuildServiceProvider();

      var ctx = new LocalContext();

      var req = new Request();
      req.Target = new Ref("Demo", "SayHi", new { Name = "Guga Coder" });

      var res = new LocalResponse();

      var pipeline = new DesignRenderingPipeline(services);
      var task = pipeline.RenderAsync(ctx, req, res, default);

      task.Wait();

      Debug.WriteLine(res);
    }

    public static void Main(string[] args) { try { Do(args); } catch (Exception ex) { Debug.WriteLine("FAIL!!!"); Debug.WriteLine(ex.GetCauseMessage()); Debug.WriteLine(ex.GetStackTrace()); } }
  }
}