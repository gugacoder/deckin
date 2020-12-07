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
using Keep.Hosting.Core;
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

namespace Innkeeper
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

      services.AddInnkeeper();
      services.AddInnkeeperJobs();
      services.AddInnkeeperSqlJobs();

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

      //app.UsePaperStaticFiles(options => options.UseDefaultFiles());

      app.UseRouting();

      //app.UsePaperApi();
      //app.UsePaperAuthentication();

      app.UseEndpoints(endpoints =>
      {
        endpoints.MapControllers();
        //endpoints.MapPapers();
      });
    }
  }
}