using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Toolset.Serialization
{
  public class DocumentEventArgs : EventArgs
  {
    public DocumentEventArgs(DocumentModel document)
    {
      this.Document = document;
    }

    public DocumentModel Document { get; }
  }

  public sealed class DocumentWriter : Writer
  {
    public event EventHandler<DocumentEventArgs> DocumentWrite;

    private Stack<NodeModel> stack;
    private Stack<Stack<NodeModel>> cache;

    private readonly List<DocumentModel> documents2 = new List<DocumentModel>();

    public DocumentWriter()
      : base(new SerializationSettings(), TextCase.KeepOriginal)
    {
      this.stack = new Stack<NodeModel>();
      this.cache = new Stack<Stack<NodeModel>>();
    }

    public DocumentWriter(SerializationSettings settings)
      : base(settings ?? new SerializationSettings(), TextCase.KeepOriginal)
    {
      this.stack = new Stack<NodeModel>();
      this.cache = new Stack<Stack<NodeModel>>();
    }

    public ICollection<DocumentModel> TargetDocuments => documents2;

    private void AddTargetDocument(DocumentModel document)
    {
      documents2.Add(document);
      DocumentWrite?.Invoke(this, new DocumentEventArgs(document));
    }

    protected override void DoWrite(Node node)
    {
      switch (node.Type)
      {
        case NodeType.DocumentStart:
          {
            var obj = new DocumentModel { SerializationValue = node.Value };
            stack.Push(obj);
            cache.Push(stack);
            stack = new Stack<NodeModel>();
            break;
          }

        case NodeType.DocumentEnd:
          {
            var root = stack.SingleOrDefault<NodeModel>();
            stack = cache.Pop();

            var document = (DocumentModel)stack.Pop();
            document.Root = root;

            AddTargetDocument(document);
            break;
          }

        case NodeType.ObjectStart:
          {
            var obj = new ObjectModel { SerializationValue = node.Value };
            stack.Push(obj);
            cache.Push(stack);
            stack = new Stack<NodeModel>();
            break;
          }

        case NodeType.ObjectEnd:
          {
            var properties = stack.Cast<PropertyModel>();
            stack = cache.Pop();

            var obj = (ObjectModel)stack.Peek();
            obj.AddPropertyRange(properties.Reverse());

            if (!cache.Any() && (stack.Count <= 1))
            {
              var document = new DocumentModel { Root = obj };
              AddTargetDocument(document);
            }

            break;
          }

        case NodeType.CollectionStart:
          {
            var collection = new CollectionModel { SerializationValue = node.Value };
            stack.Push(collection);
            cache.Push(stack);
            stack = new Stack<NodeModel>();
            break;
          }

        case NodeType.CollectionEnd:
          {
            var items = stack;
            stack = cache.Pop();

            var obj = (CollectionModel)stack.Peek();
            obj.AddRange(items.Reverse());

            break;
          }

        case NodeType.PropertyStart:
          {
            var property = new PropertyModel { SerializationValue = node.Value };
            stack.Push(property);
            cache.Push(stack);
            stack = new Stack<NodeModel>();
            break;
          }

        case NodeType.PropertyEnd:
          {
            NodeModel value = null;
            if (stack.Count == 0)
              value = new ValueModel { Value = null };
            else if (stack.Count == 1)
              value = stack.Pop();
            else
              throw new Exception("Property cannot have more than one value.");

            stack = cache.Pop();
            var property = (PropertyModel)stack.Peek();
            property.Value = value;

            break;
          }

        case NodeType.Value:
          {
            var value = new ValueModel { Value = node.Value };
            stack.Push(value);
            break;
          }
      }
    }

    protected override void DoWriteComplete()
    {
      // nada a fazer...
    }

    protected override void DoFlush()
    {
      // nada a fazer...
    }

    protected override void DoClose()
    {
      // nada a fazer...
    }
  }
}
