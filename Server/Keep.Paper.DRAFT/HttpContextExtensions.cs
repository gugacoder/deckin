﻿using System;
using System.Threading.Tasks;
using Keep.Paper.Api.Types;
using Keep.Tools;
using Keep.Tools.IO;
using Microsoft.AspNetCore.Http;

namespace Keep.Paper
{
  public static class HttpContextExtensions
  {
    public static async Task SendJsonAsync(this HttpResponse res, object data)
    {
      res.ContentType = "Content-Type: text/json; charset=UTF-8";
      var json = Json.ToJson(data);
      await res.Body.WriteAllTextAsync(json);
    }
  }
}
