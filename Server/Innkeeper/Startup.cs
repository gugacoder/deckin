using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Keep.Paper.Api;
using Keep.Paper.Api.Types;
using Keep.Paper.Configurations;
using Keep.Paper.Security;
using Keep.Paper.Services;
using Keep.Tools;
using Keep.Tools.Collections;
using Keep.Tools.IO;
using Keep.Tools.Web;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;

namespace AppSuite
{
  public class Startup
  {
    private IConfiguration configuration;

    public Startup(IConfiguration configuration)
    {
      this.configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
      services.AddControllers();
      services.AddPapers();

      services.AddCors(o => o.AddPolicy("AllowCorsPolicy", builder => builder
        .AllowAnyOrigin()
        .AllowAnyHeader()
        .AllowAnyMethod()
      ));
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
      }

      app.UseCors("AllowCorsPolicy");

      app.UsePaperStaticFiles(options => options.UseDefaultFiles());

      app.UseRouting();

      app.Use(async (context, next) =>
      {
        try
        {
          await next.Invoke();

          var code = context.Response.StatusCode;
          if (code != 200)
          {
            var status = (HttpStatusCode)code;
            var res = context.Response;
            await SendDataAsync(res, new Status
            {
              Props = new Status.Info
              {
                Fault = Fault.GetFaultForStatus(status),
                Severity =
                  (code > 500) ? Severity.Danger :
                  (code > 400) ? Severity.Warning : Severity.Default
              }
            });
          }
        }
        catch (Exception ex)
        {
          var res = context.Response;

          res.StatusCode = StatusCodes.Status500InternalServerError;
          res.ContentType = "Content-Type: text/json; charset=UTF-8";

          await SendDataAsync(res, new Status
          {
            Props = new Status.Info
            {
              Fault = Fault.Failure,
              Reason = ex.Message,
              Severity = Severity.Danger,
#if DEBUG
              StackTrace = ex.GetStackTrace()
#endif
            }
          });
        }
      });

      app.Use(async (context, next) =>
      {
        if (context.User?.Identity.IsAuthenticated == true)
        {
          await next.Invoke();
          return;
        }

        string type = null;
        string credentials = null;

        var authorizationHeader = context.Request.Headers[HeaderNames.Authorization];
        var authorization = authorizationHeader.FirstOrDefault();
        if (authorization != null)
        {
          var parts = authorization.Split(' ').NotNullOrEmpty();
          type = parts.First();
          credentials = parts.Skip(1).FirstOrDefault();
        }

        switch (type)
        {
          case "Bearer":
            {
              var jwtToken = TokenParser.ParseJwtToken(credentials);

              var identity = new ClaimsIdentity(jwtToken.Claims);
              var principal = new ClaimsPrincipal(identity);

              context.User = principal;

              await next.Invoke();
              return;
            }

          case "Basic":
            {
              credentials = Encoding
                .GetEncoding("iso-8859-1")
                .GetString(Convert.FromBase64String(credentials));

              var tokens = credentials.Split(':');
              var username = tokens.First();
              var password = tokens.Skip(1).FirstOrDefault();

              string domain = null;
              if (username.Contains("/"))
              {
                tokens = username.Split('/');
                domain = tokens.First();
                username = tokens.Last();
              }

              var builder = new TokenBuilder();
              builder.AddUserId(username);
              builder.AddUserDomain(domain);

              var jwtToken = builder.BuildJwtToken();
              var jwtTokenString = TokenBuilder.ToString(jwtToken);

              var identity = new ClaimsIdentity(jwtToken.Claims);
              var principal = new ClaimsPrincipal(identity);

              context.User = principal;
              context.Response.Headers[PaperHeaderNames.SetAuthorization] =
                $"Bearer {jwtTokenString}";

              await next.Invoke();
              return;
            }

          default:
            {
              var res = context.Response;

              res.StatusCode = StatusCodes.Status401Unauthorized;
              res.Headers[HeaderNames.WWWAuthenticate] =
                $@"Basic realm=""{App.Title}"", charset=""ISO-8859-1""";

              await SendDataAsync(res, new Status
              {
                Props = new Status.Info
                {
                  Fault = Fault.Unauthorized,
                  Reason = "Autentique-se para usar este recurso.",
                  Severity = Severity.Danger
                }
              });

              return;
            }
        }
      });

      //app.UsePaperAuthentication();
      //app.UsePaperAuthorization();

      app.UseEndpoints(endpoints =>
      {
        endpoints.MapControllers();
        endpoints.MapPapers();
      });
    }

    private async Task SendDataAsync(HttpResponse res, object data)
    {
      res.ContentType = "Content-Type: text/json; charset=UTF-8";
      var json = Json.ToJson(data);
      await res.Body.WriteAllTextAsync(json);
    }
  }
}