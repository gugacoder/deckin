﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Server.Model;

namespace Server.Controllers
{
  [Route("/Api/1/Entities", Order = 999)]
  [Route("/!", Order = 999)]
  public class EntitiesController : Controller
  {
    //[Route("{catalog}/{typeName}")]
    //public IActionResult Index(string catalog, string typeName, string key)

    [Route("{catalog}/{typeName}/{key}")]
    public IActionResult Retrieve(string catalog, string typeName, string key)
    {
      try
      {
        var type = Type.GetType($"{typeName}, {catalog}");
        if (type == null)
        {
          return NotFound(new
          {
            Type = "status",
            Data = new
            {
              Status = (int)HttpStatusCode.NotFound,
              Message = HttpStatusCode.NotFound.ToString(),
              RequestUri = this.Request.GetDisplayUrl()
            },
            Links = new
            {
              Self = new
              {
                Href = this.Request.GetDisplayUrl()
              }
            }
          });
        }

        var getter = type.GetMethod("Retrieve");
        if (getter == null)
        {
          return NotFound(new
          {
            Type = "status",
            Data = new
            {
              Status = (int)HttpStatusCode.NotFound,
              Message = HttpStatusCode.NotFound.ToString(),
              RequestUri = this.Request.GetDisplayUrl()
            },
            Links = new
            {
              Self = new
              {
                Href = this.Request.GetDisplayUrl()
              }
            }
          });
        }

        var parser = type.GetMethod("ParseKey");
        var id = parser?.Invoke(null, new[] { key }) ?? key;

        var result = getter.Invoke(null, new[] { id });
        if (result == null)
        {
          return NotFound(new
          {
            Type = "status",
            Data = new
            {
              Status = (int)HttpStatusCode.NotFound,
              Message = HttpStatusCode.NotFound.ToString(),
              RequestUri = this.Request.GetDisplayUrl()
            },
            Links = new
            {
              Self = new
              {
                Href = this.Request.GetDisplayUrl()
              }
            }
          });
        }

        return Ok(new
        {
          Type = "record",
          Data = result,
          Links = new
          {
            Self = new
            {
              Href = this.Request.GetDisplayUrl()
            }
          }
        });
      }
      catch (Exception ex)
      {
        return StatusCode((int)HttpStatusCode.InternalServerError, new
        {
          Type = "status",
          Data = new
          {
            Status = (int)HttpStatusCode.InternalServerError,
            Message = HttpStatusCode.InternalServerError.ToString(),
            RequestUri = this.Request.GetDisplayUrl(),
            Fault = new
            {
              Message = EnumerateCauses(ex).Select(x => x.Message).ToArray(),
              StackTrace = string.Join("\n- - -\n", EnumerateCauses(ex).Select(x => x.StackTrace))
            }
          },
          Links = new
          {
            Self = new
            {
              Href = this.Request.GetDisplayUrl()
            }
          }
        });
      }
    }

    private IEnumerable<Exception> EnumerateCauses(Exception ex)
    {
      while (ex != null)
      {
        yield return ex;
        ex = ex.InnerException;
      }
    }
  }
}