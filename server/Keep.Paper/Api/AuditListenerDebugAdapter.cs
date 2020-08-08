using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Keep.Tools;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Keep.Paper.Api
{
  public class AuditListenerDebugAdapter : IAuditListener
  {
    public void Audit(Level level, Type source, string @event, string message)
    {
      var text = string.Join(Environment.NewLine,
        message.Split('\n').Select(x => $"  {x}"));

      var kind = level.ToString().ToUpper();
      var date = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
      var type = source.FullName.Split(',', ';').First();
      var template = "[{0}] {1} {2} {3}:\n{4}";

      Debug.WriteLine(template, kind, date, type, @event, text);
    }
  }
}
