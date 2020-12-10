using System;
using System.Reflection;
using System.Threading.Tasks;

namespace Keep.Paper.Design
{
  public interface IPaperInterceptor
  {
    Task<object> InterceptPaper(PaperInfo info, NextAsync nextAsync);
  }
}
