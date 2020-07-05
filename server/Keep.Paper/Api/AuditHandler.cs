using System;
using Microsoft.Extensions.Logging;

namespace Keep.Paper.Api
{
  public delegate void AuditHandler(Level level, string message, Type source, string @event);
}
