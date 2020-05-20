using System;
namespace Keep.Paper
{
  public interface IPaperContext
  {
    IRequest Request { get; }

    IResponse Response { get; }
  }
}
