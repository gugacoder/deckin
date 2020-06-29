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
    public const string ApiPrefix = "/Api/1/Papers";

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

    public static string To(HttpContext ctx, string catalogName,
        string paperName, string actionName, params object[] actionKeys)
    {
      return CreateHref(ctx, catalogName, paperName, actionName, actionKeys);
    }

    public static string To(HttpContext ctx, string catalogName,
        string paperName, string actionName, string actionKeys)
    {
      return CreateHref(ctx, catalogName, paperName, actionName, actionKeys);
    }

    #endregion

    #region To Paper Specification

    public static string To(HttpContext ctx, Type type, string action = "Index")
    {
      var catalog = Name.Catalog(type);
      var paper = Name.Paper(type);
      return CreateHref(ctx, catalog, paper, action, null);
    }

    public static string To(HttpContext ctx, string catalog, string paper)
    {
      return CreateHref(ctx, catalog, paper, null, null);
    }

    #endregion

    private static string CreateHref(HttpContext ctx, string catalogName,
        string paperName, string actionName, params object[] actionKeys)
    {
      var builder = new UriBuilder(ctx.Request.GetDisplayUrl());
      builder.Scheme = null;
      builder.Host = null;
      builder.Query = null;
      builder.Path = ApiPrefix;

      if (!string.IsNullOrEmpty(catalogName)) builder.Path += $"/{catalogName}";
      if (!string.IsNullOrEmpty(paperName)) builder.Path += $"/{paperName}";
      if (!string.IsNullOrEmpty(actionName)) builder.Path += $"/{actionName}";
      if (actionKeys?.Any() == true)
      {
        builder.Path += $"/";
        builder.Path += string.Join(";", actionKeys.Select(Change.To<string>));
      }

      var url = builder.ToString();
      return url;
    }
  }
}
