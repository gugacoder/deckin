using System;
using Keep.Hosting.Extensions;
using Keep.Tools;
using Keep.Tools.Collections;

namespace Keep.Paper.Design.Rendering
{
  public class RendererCollection : Collection<IDesignRenderer>
  {
    public RendererCollection(IServiceProvider services)
    {
      ImportExposedRenderers(services);
    }

    private void ImportExposedRenderers(IServiceProvider services)
    {
      try
      {
        var types = ExposedTypes.GetTypes<IDesignRenderer>();
        foreach (var type in types)
        {
          try
          {
            var renderer = (IDesignRenderer)services.Instantiate(type);
            Add(renderer);
          }
          catch (Exception ex)
          {
            ex.Trace();
          }
        }
      }
      catch (Exception ex)
      {
        ex.Trace();
      }
    }
  }
}
