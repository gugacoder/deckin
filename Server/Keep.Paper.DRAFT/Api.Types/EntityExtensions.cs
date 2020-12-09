using System;
using Keep.Tools;
using Newtonsoft.Json;

namespace Keep.Hosting.Api.Types
{
  public static class EntityExtensions
  {
    public static string ToJson(this Entity entity, int indentation = default)
    {
      return Json.ToJson(entity, indentation);
    }
  }
}
