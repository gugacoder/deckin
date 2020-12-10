using System;
using System.Reflection;
using System.Threading.Tasks;

namespace Keep.Paper.Design
{
  public delegate Task<object> NextAsync(PaperInfo info);
}