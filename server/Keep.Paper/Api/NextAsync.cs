using System;
using System.Reflection;
using System.Threading.Tasks;

namespace Keep.Paper.Api
{
  public delegate Task<object> NextAsync(PaperInfo info);
}