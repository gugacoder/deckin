using System;
using System.Reflection;
using System.Threading.Tasks;

namespace Keep.Hosting.Api
{
  public interface IPaperInterceptor
  {
    Task<object> InterceptPaper(PaperInfo info, NextAsync nextAsync);
  }
}
