using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Threading.Tasks;
using AppSuite.Adaptadores;
using AppSuite.Conectores;
using AppSuite.Servicos;
using Keep.Paper.Api;
using Keep.Paper.Configurations;
using Keep.Paper.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

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

      services.AddSingleton<ServicoDeAuditoria>();

      services.AddCors(o => o.AddPolicy("AllowCorsPolicy", builder => builder
        .AllowAnyOrigin()
        .AllowAnyHeader()
        .AllowAnyMethod()
      ));

      services.AddTransient<DbDirector>();
      services.AddTransient<DbConcentrador>();
      services.AddTransient<DbPdv>();
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

      //app.UsePaperAuthentication();
      //app.UsePaperAuthorization();

      app.UseEndpoints(endpoints =>
      {
        endpoints.MapControllers();
        endpoints.MapPapers();
      });
    }
  }
}