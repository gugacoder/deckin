using System;
using System.Collections.Generic;
using System.Linq;
using Keep.Tools;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Keep.Hosting.Api
{
  internal class AuditListenerLoggerAdapter : IAuditListener
  {
    private readonly ILogger<AuditListenerLoggerAdapter> logger;

    public AuditListenerLoggerAdapter(ILogger<AuditListenerLoggerAdapter> logger)
    {
      this.logger = logger;
    }

    public void Audit(Level level, Type source, string @event, string message)
    {
      LogLevel logLevel;
      switch (level)
      {
        case Level.Trace:
          logLevel = LogLevel.Trace;
          break;
        case Level.Warning:
          logLevel = LogLevel.Warning;
          break;
        case Level.Danger:
          logLevel = LogLevel.Critical;
          break;
        default:
          logLevel = LogLevel.Information;
          break;
      }

      var text = string.Join(Environment.NewLine,
        message.Split('\n').Select(x => $"  {x}"));

      var kind = level.ToString().ToUpper();
      var date = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
      var type = source.FullName.Split(',', ';').First();
      var template = "[{0}] {1} {2} {3}:\n{4}";

      logger.Log(logLevel, template, kind, date, type, @event, text);
    }
  }
}
