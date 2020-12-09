using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Keep.Hosting.Api;
using Keep.Tools;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Keep.Hosting.Jobs
{
  [Expose]
  public class ExposedJobFactory : IJobFactoryAsync
  {
    private readonly IConfiguration configuration;
    private readonly IServiceProvider provider;
    private readonly IAudit<ExposedJobFactory> audit;

    public ExposedJobFactory(IConfiguration configuration,
      IServiceProvider provider, IAudit<ExposedJobFactory> audit)
    {
      this.configuration = configuration;
      this.provider = provider;
      this.audit = audit;
    }

    public Task AddJobsAsync(IJobScheduler jobScheduler)
    {
      try
      {
        var types = ExposedTypes.GetTypes<IJobAsync>();
        types = FilterJobs(types);
        foreach (var type in types)
        {
          try
          {
            var job = (IJobAsync)ActivatorUtilities.CreateInstance(provider, type);
            job.SetUp(jobScheduler);
            Console.WriteLine($"[JOB_ADDED]{GetTypeName(job.GetType())}");
          }
          catch (Exception ex)
          {
            audit.LogDanger(To.Text(ex));
          }
        }
      }
      catch (Exception ex)
      {
        audit.LogDanger(To.Text(ex));
      }
      return Task.CompletedTask;
    }

    private IEnumerable<Type> FilterJobs(IEnumerable<Type> types)
    {
      var jobs = configuration.GetSection("Host:Jobs");
      var settings = (
        from child in jobs.GetChildren()
        let key = child.Key
        let enabled = child["Enabled"]
        select new
        {
          key,
          enabled = Change.To(enabled ?? "true", false)
        }
      ).ToArray();

      var allTypes = types.ToArray();
      var enabledTypes = types.ToList();

      foreach (var setting in settings)
      {
        try
        {
          var key = setting.key;
          if (!key.Contains("."))
          {
            key = $"*.{key}";
          }
          var pattern = key.Replace("*", ".*");
          var regex = new Regex(pattern);

          if (setting.enabled)
          {
            var matchedTypes = allTypes.Where(type =>
            {
              var name = type.FullName.Split(',', ';').First();
              return regex.IsMatch(GetTypeName(type));
            });
            enabledTypes.AddRange(matchedTypes.Except(enabledTypes));
          }
          else
          {
            enabledTypes.RemoveAll(type => regex.IsMatch(GetTypeName(type)));
          }
        }
        catch (Exception ex)
        {
          ex.Trace();
        }
      }

      return enabledTypes;
    }

    private string GetTypeName(Type type)
    {
      return type.FullName.Split(',', ';').First();
    }
  }
}
