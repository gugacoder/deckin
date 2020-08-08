using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Keep.Paper.Api;
using Keep.Paper.Configurations;
using Keep.Paper.Services;
using Keep.Tools;
using Keep.Tools.Collections;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

#nullable enable

namespace Director
{
  public class Program
  {
    public static void Main(string[] args)
    {
      try
      {
        //var auditSettings = AuditSettings.CreateDefault();
        //auditSettings.AddListener(new AuditListenerDebugAdapter());

        //var audit = new Audit<CommonSettings>(auditSettings);
        //var settings = new CommonSettings(null);

        //foreach (var key in settings.Keys)
        //{
        //  Debug.WriteLine(settings.Get(key));
        //}

        //settings.Set(" MY  .   ID    ", 1);
        //settings.Set("my .name ", "One");
        //settings.Set(" My. DATE  ", DateTime.Now);

        //Debug.WriteLine(settings.Get<int>("My . ID"));
        //Debug.WriteLine(settings.Get<string>("My . NAME"));
        //Debug.WriteLine(settings.Get<DateTime>("  My . DATE  "));
      }
      catch (Exception ex)
      {
        ex.Trace();
      }
    }
  }
}
