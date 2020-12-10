using System;
namespace Keep.Paper.Api
{
  public interface IAuditListener
  {
    void Audit(Level level, Type source, string @event, string message);
  }
}
