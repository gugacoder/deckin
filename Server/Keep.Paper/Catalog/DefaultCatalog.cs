﻿using System;
using Microsoft.Extensions.DependencyInjection;

namespace Keep.Paper.Catalog
{
  public class DefaultCatalog : CompositeCatalog
  {
    public DefaultCatalog(IServiceProvider provider)
    {
      var embeddedCatalog =
        ActivatorUtilities.CreateInstance<EmbeddedCatalog>(provider);

      this.Add(embeddedCatalog);
    }
  }
}