using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Keep.Tools;
using Keep.Tools.Collections;

namespace Keep.Paper.Formatters
{
  public static class Entities
  {
    public static string GetType(object @object)
    {
      if (@object is int @int) @object = (HttpStatusCode)@int;

      if (@object is Exception)
        return "fault";

      if (@object is HttpStatusCode)
        return "status";

      if (Is.Anonymous(@object))
        return "entity";

      var type = (@object as Type) ?? @object.GetType();
      return type.FullName.Split(',').First();
    }

    public static object GetData(params object[] objects)
    {
      var data = new HashMap();
      var strings = objects.OfType<string>();

      if (strings.Any())
      {
        data.Add(new
        {
          messages = strings.ToArray()
        });
      }

      objects.Except(strings).ForEach(@object =>
      {
        if (@object is int @int) @object = (HttpStatusCode)@int;

        if (@object is Exception exception)
        {
          var messages = EnumerateCauses(exception).
              Select(x => x.Message).ToArray();
          var traceInfo = "FAULT: " + string.Join("\nCAUSE: ",
              EnumerateCauses(exception).
              Select(x => $"{x.Message}\n{x.StackTrace}"));

          data.Add(new
          {
            fault = GetType(exception),
            faultDescription = messages,
            faultTrace = traceInfo
          });

          return;
        }

        if (@object is HttpStatusCode status)
        {
          data.Add(new
          {
            status = (int)status,
            statusDescription = status.ToString().ChangeCase(TextCase.ProperCase)
          });
          return;
        }

        data.Add(@object);
      });

      return data;
    }

    private static IEnumerable<Exception> EnumerateCauses(Exception exception)
    {
      while (exception != null)
      {
        yield return exception;
        exception = exception.InnerException;
      }
    }
  }
}
