using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Keep.Paper.DesignX.Modeling;
using Keep.Tools;
using Keep.Tools.Collections;

namespace Keep.Paper.DesignX.Rendering
{
  public class Action
  {
    public Resource Resource { get; set; }
    public ActionBatch[] Batches { get; set; }
    public ContentType[] AcceptedContentType { get; set; }
  }
}