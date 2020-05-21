using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Keep.Paper.Formatters;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace Keep.Paper.Controllers
{
  [Route("/Api/1/Entities")]
  [Route("/!")]
  public class EntitiesController : Controller
  {
    [Route("{catalog}/{typeName}")]
    public IActionResult Index(string catalog, string typeName)
    {
      try
      {
        var type = Type.GetType($"{typeName}, {catalog}");
        if (type == null)
        {
          var status = HttpStatusCode.NotFound;
          return StatusCode((int)status, new
          {
            Type = Entities.GetType(status),
            Data = Entities.GetData(status),
            Links = new
            {
              Self = new
              {
                Href = this.Request.GetDisplayUrl()
              }
            }
          });
        }

        var getter = type.GetMethod("Index");
        if (getter == null)
        {
          var status = HttpStatusCode.NotFound;
          return StatusCode((int)status, new
          {
            Type = Entities.GetType(status),
            Data = Entities.GetData(status),
            Links = new
            {
              Self = new
              {
                Href = this.Request.GetDisplayUrl()
              }
            }
          });
        }

        var result = getter.Invoke(null, new object[] { null });
        if (result == null)
        {
          var status = HttpStatusCode.NotFound;
          return StatusCode((int)status, new
          {
            Type = Entities.GetType(status),
            Data = Entities.GetData(status),
            Links = new
            {
              Self = new
              {
                Href = this.Request.GetDisplayUrl()
              }
            }
          });
        }

        var keyGetter = type.GetMethod("GetKey");
        if (keyGetter == null)
        {
          var status = HttpStatusCode.InternalServerError;
          return StatusCode((int)status, new
          {
            Type = Entities.GetType(status),
            Data = Entities.GetData(status,
                "O objeto não declara uma função `GetKey(target:object):object`"),
            Links = new
            {
              Self = new
              {
                Href = this.Request.GetDisplayUrl()
              }
            }
          });
        }

        var records = ((IEnumerable)result).Cast<object>()
            .Select(record =>
            {
              var key = keyGetter.Invoke(null, new[] { record });
              return new
              {
                Type = Entities.GetType(record),
                Data = Entities.GetData(record),
                Links = new
                {
                  Self = new
                  {
                    Href = this.Request.GetDisplayUrl()
                  }
                }
              };
            })
            .ToArray();

        return Ok(new
        {
          Type = Entities.GetType(records),
          Entities = Entities.GetData(records),
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
          Type = Entities.GetType(ex),
          Data = Entities.GetData(ex),
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

    [Route("{catalog}/{typeName}/{key}")]
    public IActionResult Retrieve(string catalog, string typeName, string key)
    {
      try
      {
        var type = Type.GetType($"{typeName}, {catalog}");
        if (type == null)
        {
          var status = HttpStatusCode.NotFound;
          return StatusCode((int)status, new
          {
            Type = Entities.GetType(status),
            Data = Entities.GetData(status),
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
          var status = HttpStatusCode.NotFound;
          return StatusCode((int)status, new
          {
            Type = Entities.GetType(status),
            Data = Entities.GetData(status),
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

        var record = getter.Invoke(null, new[] { id });
        if (record == null)
        {
          var status = HttpStatusCode.NotFound;
          return StatusCode((int)status, new
          {
            Type = Entities.GetType(status),
            Data = Entities.GetData(status),
            Links = new
            {
              Self = new
              {
                Href = this.Request.GetDisplayUrl()
              }
            }
          });
        }

        var parent = new Uri(new Uri(this.Request.GetDisplayUrl()), "..");
        var linkToAll = new Uri(parent, typeName);

        return Ok(new
        {
          Type = Entities.GetType(record),
          Entities = Entities.GetData(record),
          Links = new
          {
            Self = new
            {
              Href = this.Request.GetDisplayUrl()
            },
            All = new
            {
              Href = linkToAll
            }
          }
        });
      }
      catch (Exception ex)
      {
        return StatusCode((int)HttpStatusCode.InternalServerError, new
        {
          Type = Entities.GetType(ex),
          Data = Entities.GetData(ex),
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