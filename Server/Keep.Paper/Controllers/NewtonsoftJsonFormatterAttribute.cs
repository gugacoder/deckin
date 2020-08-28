using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Buffers;

namespace Keep.Paper.Controllers
{
  [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
  public class NewtonsoftJsonFormatterAttribute : ActionFilterAttribute
  {
    public override void OnActionExecuted(ActionExecutedContext context)
    {
      if (context.Result is ObjectResult result)
      {
        result.Formatters.Add(new NewtonsoftJsonOutputFormatter(
          new JsonSerializerSettings
          {
            NullValueHandling = NullValueHandling.Ignore,
            DateFormatHandling = DateFormatHandling.IsoDateFormat,
            ContractResolver = new DefaultContractResolver
            {
              NamingStrategy = new PaperNamingStrategy()
            }
          },
          context.HttpContext.RequestServices.GetRequiredService<ArrayPool<char>>(),
          new MvcOptions()
        ));
      }
    }

    public class PaperNamingStrategy : CamelCaseNamingStrategy
    {
      public override string GetPropertyName(string name, bool hasSpecifiedName)
      {
        // Campos como DFid_tal são serializados como estão.
        return name.StartsWith("DF")
          ? name : base.GetPropertyName(name, hasSpecifiedName);
      }
    }
  }
}