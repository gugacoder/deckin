using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Keep.Paper.Api;
using Keep.Paper.Formatters;
using Keep.Paper.Services;
using Keep.Tools;
using Keep.Tools.Reflection;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace Keep.Paper.Controllers
{
  [Route("/Api/1/Papers")]
  [Route("/!")]
  public class PaperController : Controller
  {
    private readonly IServiceProvider serviceProvider;
    private readonly IPaperTypeCollection paperTypeCollection;

    public PaperController(IServiceProvider serviceProvider,
        IPaperTypeCollection paperTypeCollection)
    {
      this.serviceProvider = serviceProvider;
      this.paperTypeCollection = paperTypeCollection;
    }

    [Route("{catalogName}/{paperName}")]
    public async Task<IActionResult> ResolveAsync(string catalogName,
        string paperName)
    {
      return await ResolveAsync(catalogName, paperName, "Index");
    }

    [Route("{catalogName}/{paperName}/{actionName}")]
    public async Task<IActionResult> ResolveAsync(string catalogName,
        string paperName, string actionName)
    {
      try
      {
        var paperType = paperTypeCollection.FindPaperType(catalogName, paperName);
        if (paperType == null)
          return SendNotFound(Request.Path.Value);

        var resolveMethod = paperType.GetMethod(actionName)
            ?? paperType.GetMethod($"{actionName}Async");
        if (resolveMethod == null)
          return BadRequest(
              "O Paper existe mas não está preparado para responder uma " +
              "requisição direta. Não possui um método " +
              "`Resolve(object):object'.");

        var parameter = resolveMethod.GetParameters().FirstOrDefault();
        var parameterValue = (parameter != null)
            ? await ParseBodyAsync(Request.Body, parameter.ParameterType)
            : null;

        var paperInstance = ActivatorUtilities.CreateInstance(serviceProvider,
            paperType);

        var args = (parameter != null) ? new[] { parameterValue } : null;
        var result = await resolveMethod.InvokeAsync(paperInstance, args);
        if (result == null)
          return SendNotFound(Request.Path.Value);

        return Ok(result);
      }
      catch (Exception ex)
      {
        return NotFound(new
        {
          Kind = Kinds.Fault,
          Data = new
          {
            Status = 500,
            StatusDescription = "Falha Processando a Requisição",
            Cause = ex.GetCauseMessages()
          },
          Links = new
          {
            Self = new { Href = Names.Get(catalogName, paperName, actionName) }
          }
        });
      }
    }

    [Route("{*path}")]
    public IActionResult SendNotFound(string path)
    {
      return NotFound(new
      {
        Kind = Kinds.Fault,
        Data = new
        {
          Status = 404,
          StatusDescription = "Não Encontrado"
        },
        Links = new
        {
          Self = new { Href = Names.Get(path) }
        }
      });
    }

    private async Task<object> ParseBodyAsync(Stream body, Type parameterType)
    {
      using var reader = new StreamReader(body);
      var json = await reader.ReadToEndAsync();
      if (string.IsNullOrWhiteSpace(json)) json = "{}";
      var @object = JsonConvert.DeserializeObject(json, parameterType);
      return @object;
    }
  }
}