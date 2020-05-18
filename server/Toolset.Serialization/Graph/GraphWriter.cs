using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Toolset.Serialization.Graph
{
  public class GraphWriter<T> : GraphWriter
    where T : class, new()
  {
    public GraphWriter()
      : base(typeof(T), new GraphSerializationSettings())
    {
      // nada a fazer aqui. use o outro construtor.
    }

    public GraphWriter(SerializationSettings settings)
      : base(typeof(T), settings)
    {
      // nada a fazer aqui. use o outro construtor.
    }

    public new IEnumerable<T> Graphs => base.Graphs.OfType<T>();
  }

  public class GraphWriter : Writer
  {
    private readonly Type graphType;
    private readonly List<object> graphList;
    private readonly DocumentWriter writer;

    public GraphWriter(Type graphType)
      : this(graphType, new GraphSerializationSettings())
    {
      // Nada a fazer neste construtor.
    }

    public GraphWriter(Type graphType, SerializationSettings settings)
      : base(
          settings as GraphSerializationSettings ?? new GraphSerializationSettings(settings),
          TextCase.KeepOriginal
        )
    {
      this.graphType = graphType;
      this.graphList = new List<object>();
      this.writer = new DocumentWriter();
      this.writer.DocumentWrite += (o, e) => DeserializeGraph(e.Document);
    }

    public new GraphSerializationSettings Settings => (GraphSerializationSettings)base.Settings;

    public IEnumerable<object> Graphs => graphList;

    protected override void DoWrite(Node node)
    {
      writer.Write(node);
    }

    protected override void DoWriteComplete()
    {
      writer.WriteComplete();
    }

    protected override void DoFlush()
    {
      writer.Flush();
    }

    protected override void DoClose()
    {
      writer.Close();
    }

    private void DeserializeGraph(DocumentModel document)
    {
      var builder = new GraphBuilder();
      var graph = builder.CreateGraph(document, graphType);
      graphList.Add(graph);
    }
  }
}
