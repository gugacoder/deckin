﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Keep.Tools.Collections;
using Keep.Tools.Web;
using Keep.Tools.Xml;

namespace Keep.Tools
{
  public struct Ret
  {
    public bool Ok => (int)Status.Code < 400;

    public object Value { get; set; }

    public RetStatus Status { get; set; }

    public RetFault Fault { get; set; }

    public override string ToString()
    {
      var fault = Fault.ToString();
      return (fault.Length > 0) ? $"{Status} - {fault}" : Status.ToString();
    }

    public static implicit operator bool(Ret ret)
    {
      return ret.Value is bool bit ? bit : ret.Ok;
    }

    public static implicit operator Ret(bool value)
    {
      return new Ret
      {
        Status = new RetStatus
        {
          Code = value ? HttpStatusCode.OK : HttpStatusCode.NotFound
        }
      };
    }

    public static implicit operator Ret(Exception exception)
    {
      exception.Trace();
      return new Ret
      {
        Status = new RetStatus
        {
          Code = HttpStatusCode.InternalServerError
        },
        Fault = new RetFault
        {
          Message = exception.GetCauseMessage(),
          Exception = exception
        }
      };
    }

    #region Estruturas

    public struct RetStatus
    {
      private HashMap<string> _headers;

      public HttpStatusCode Code { get; set; }

      public int CodeValue
      {
        get => (int)Code;
        set => Code = (HttpStatusCode)value;
      }

      public HttpStatusClass CodeClass
      {
        get => (HttpStatusClass)(CodeValue / 100);
      }

      public HashMap<string> Headers
      {
        get => _headers ?? (_headers = new HashMap<string>());
        set => _headers = value;
      }

      public override string ToString()
      {
        return $"{(int)Code} - {Code}";
      }
    }

    public struct RetFault
    {
      private string _message;

      public string Message
      {
        get => _message;
        set => _message = !string.IsNullOrWhiteSpace(value) ? value.Trim() : null;
      }

      public Exception Exception { get; set; }

      public override string ToString()
      {
        return Message ?? Exception?.Message;
      }
    }

    #endregion

    #region Extensões

    public static Ret OK()
    {
      return new Ret
      {
        Status = new RetStatus
        {
          Code = HttpStatusCode.OK
        }
      };
    }

    public static Ret<T> OK<T>(T value)
    {
      return new Ret<T>
      {
        Value = value,
        Status = new RetStatus
        {
          Code = HttpStatusCode.OK
        }
      };
    }

    public static Ret NotFound()
    {
      return new Ret
      {
        Status = new RetStatus
        {
          Code = HttpStatusCode.NotFound
        }
      };
    }

    public static Ret<T> NotFound<T>(T path)
    {
      return new Ret<T>
      {
        Value = path,
        Status = new RetStatus
        {
          Code = HttpStatusCode.NotFound
        }
      };
    }

    public static Ret<T> Create<T>(HttpStatusCode status, T value)
    {
      return new Ret<T>
      {
        Value = value,
        Status = new RetStatus
        {
          Code = status
        }
      };
    }

    public static Ret Create(HttpStatusCode status)
    {
      return new Ret
      {
        Status = new RetStatus
        {
          Code = status
        }
      };
    }

    public static Ret Fail(HttpStatusCode status, string faultMessage)
    {
      return new Ret
      {
        Status = new RetStatus
        {
          Code = status
        },
        Fault = new RetFault
        {
          Message = faultMessage
        }
      };
    }

    public static Ret Fail(HttpStatusCode status, Exception exception)
    {
      return new Ret
      {
        Status = new RetStatus
        {
          Code = status
        },
        Fault = new RetFault
        {
          Message = exception?.GetCauseMessage(),
          Exception = exception
        }
      };
    }

    public static Ret Fail(HttpStatusCode status, string faultMessage, Exception exception)
    {
      return new Ret
      {
        Status = new RetStatus
        {
          Code = status
        },
        Fault = new RetFault
        {
          Message = faultMessage ?? exception.GetCauseMessage(),
          Exception = exception
        }
      };
    }

    public static Ret Fail(string faultMessage)
    {
      return Fail(faultMessage, null);
    }

    public static Ret Fail(Exception exception)
    {
      return Fail(null, exception);
    }

    public static Ret Fail(string faultMessage, Exception exception)
    {
      return new Ret
      {
        Status = new RetStatus
        {
          Code = HttpStatusCode.InternalServerError
        },
        Fault = new RetFault
        {
          Message = faultMessage ?? exception?.GetCauseMessage(),
          Exception = exception
        }
      };
    }

    public static Ret<T> FailWithValue<T>(HttpStatusCode status, string faultMessage, T value)
    {
      return new Ret
      {
        Value = value,
        Status = new RetStatus
        {
          Code = status
        },
        Fault = new RetFault
        {
          Message = faultMessage
        }
      };
    }

    public static Ret<T> FailWithValue<T>(HttpStatusCode status, Exception exception, T value)
    {
      return new Ret
      {
        Value = value,
        Status = new RetStatus
        {
          Code = status
        },
        Fault = new RetFault
        {
          Message = exception?.GetCauseMessage(),
          Exception = exception
        }
      };
    }

    public static Ret<T> FailWithValue<T>(HttpStatusCode status, string faultMessage, Exception exception, T value)
    {
      return new Ret
      {
        Value = value,
        Status = new RetStatus
        {
          Code = status
        },
        Fault = new RetFault
        {
          Message = faultMessage ?? exception.GetCauseMessage(),
          Exception = exception
        }
      };
    }

    public static Ret<T> FailWithValue<T>(string faultMessage, T value)
    {
      return FailWithValue<T>(faultMessage, null, value);
    }

    public static Ret<T> FailWithValue<T>(Exception exception, T value)
    {
      return FailWithValue<T>(null, exception, value);
    }

    public static Ret<T> FailWithValue<T>(string faultMessage, Exception exception, T value)
    {
      return new Ret
      {
        Value = value,
        Status = new RetStatus
        {
          Code = HttpStatusCode.InternalServerError
        },
        Fault = new RetFault
        {
          Message = faultMessage ?? exception?.GetCauseMessage(),
          Exception = exception
        }
      };
    }

    #endregion
  }
}