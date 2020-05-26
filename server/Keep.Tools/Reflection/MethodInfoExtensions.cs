using System;
using System.Reflection;
using System.Threading.Tasks;

namespace Keep.Tools.Reflection
{
  public static class MethodInfoExtensions
  {
    public static async Task<object> InvokeAsync(this MethodInfo method,
        object obj, object[] parameters)
    {
      var result = method.Invoke(obj, parameters);
      if (result is Task task)
      {
        await task.ConfigureAwait(false);

        var getter = task.GetType().GetProperty("Result");
        result = getter?.GetValue(task);
      }
      return result;
    }
  }
}
