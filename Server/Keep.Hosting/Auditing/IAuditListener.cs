using System;
namespace Keep.Hosting.Auditing
{
  public interface IAuditListener
  {
    void Audit(Level level, Type source, string @event, string message);
  }
}
