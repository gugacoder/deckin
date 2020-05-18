using System;
namespace Paper
{
  public interface IPaperContext
  {
    IRequest Request { get; }

    IResponse Response { get; }
  }
}
