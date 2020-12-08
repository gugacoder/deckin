using System;
namespace Keep.Hosting.Api
{
  public interface IAuditListener
  {
    void Audit(Level level, Type source, string @event, string message);
  }
}
