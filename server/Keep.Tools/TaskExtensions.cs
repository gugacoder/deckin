using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Keep.Tools
{
  public static class TaskExtensions
  {
    public static void NoAwait(this Task task)
    {
      // Nada a fazer. A tarefa será executada em paralelo.
    }

    public static T Await<T>(this Task<T> task)
    {
      return task.GetAwaiter().GetResult();
    }

    public static void Await(this Task task)
    {
      task.GetAwaiter().GetResult();
    }
  }
}