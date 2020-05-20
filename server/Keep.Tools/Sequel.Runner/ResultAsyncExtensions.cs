using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Keep.Tools.Sequel.Runner
{
  public static class ResultAsyncExtensions
  {
    public static async Task<object[]> ToArrayAsync(this IReaderAsync result)
    {
      var items = new List<object>();
      while (await result.ReadAsync())
      {
        items.Add(result.Current);
      }
      return items.ToArray();
    }

    public static async Task<T[]> ToArrayAsync<T>(this IReaderAsync<T> result)
    {
      var items = new List<T>();
      while (await result.ReadAsync())
      {
        items.Add(result.Current);
      }
      return items.ToArray();
    }
  }
}
