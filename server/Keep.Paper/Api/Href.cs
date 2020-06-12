using System;
using System.Linq;
using System.Runtime.CompilerServices;
using Keep.Tools;
using Keep.Tools.Collections;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;

namespace Keep.Paper.Api
{
  public static class Href
  {
    public static string MakeRelative(string href)
    {
      var builder = new UriBuilder(href);
      builder.Scheme = null;
      builder.Host = null;
      var relativeUri = builder.ToString();
      return relativeUri;
    }

    #region To Paper

    public static string To(HttpContext ctx, Type type, string method,
        params object[] keys)
    {
      var catalog = Name.Catalog(type);
      var paper = Name.Paper(type);
      var action = Name.Action(method);
      return CreateHref(ctx, catalog, paper, action, keys);
    }

    public static string To(HttpContext ctx, string catalog, string paper,
        string action, params object[] keys)
    {
      return CreateHref(ctx, catalog, paper, action, keys);
    }

    #endregion

    #region To Paper Specification

    public static string To(HttpContext ctx, Type type)
    {
      var catalog = Name.Catalog(type);
      var paper = Name.Paper(type);
      return CreateHref(ctx, catalog, paper, null, null);
    }

    public static string To(HttpContext ctx, string catalog, string paper)
    {
      return CreateHref(ctx, catalog, paper, null, null);
    }

    #endregion

    private static string CreateHref(HttpContext ctx, string catalog,
        string paper, string action, object[] keys)
    {
      var builder = new UriBuilder(ctx.Request.GetDisplayUrl());
      builder.Scheme = null;
      builder.Host = null;
      builder.Query = null;
      builder.Path = "/!";

      if (!string.IsNullOrEmpty(catalog)) builder.Path += $"/{catalog}";
      if (!string.IsNullOrEmpty(paper)) builder.Path += $"/{paper}";
      if (!string.IsNullOrEmpty(action)) builder.Path += $"/{action}";
      if (keys?.Any() == true)
      {
        builder.Path += $"/";
        builder.Path += string.Join("/", keys.Select(Change.To<string>));
      }

      var url = builder.ToString();
      return url;
    }
  }
}
