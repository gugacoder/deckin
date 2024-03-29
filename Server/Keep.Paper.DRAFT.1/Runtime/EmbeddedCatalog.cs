﻿using System;
using System.Diagnostics;
using Keep.Hosting.Auditing;
using Keep.Paper.Design;
using Keep.Tools;
using Keep.Tools.Collections;
using Microsoft.Extensions.DependencyInjection;

namespace Keep.Paper.Runtime
{
  public class EmbeddedCatalog : ICatalog
  {
    private readonly IServiceProvider provider;
    private readonly IAudit<EmbeddedCatalog> audit;
    private readonly HashMap<IAction> catalog;

    public EmbeddedCatalog(IServiceProvider provider,
      IAudit<EmbeddedCatalog> audit)
    {
      this.provider = provider;
      this.audit = audit;
      this.catalog = new HashMap<IAction>();
      LoadEmbeddedActions();
    }

    private void LoadEmbeddedActions()
    {
      var types = ExposedTypes.GetTypes<IActionFactory>();
      foreach (var type in types)
      {
        try
        {
          var factory =
            (IActionFactory)ActivatorUtilities.CreateInstance(provider, type);

          var actions = factory.CreateActionsAsync().Await();

          actions.ForEach(action => catalog.Add(action.Ref.Name, action));
#if DEBUG
          actions.ForEach(action =>
            Debug.WriteLine($"Action: /Api/1/Papers/{action.Ref}"));
#endif
        }
        catch (Exception ex)
        {
          audit.LogDanger(
            To.Text(
              $"Falhou a tentativa de carregar a ação embarcada: {type.GetType().FullName}",
              ex));
        }
      }
    }

    public IAction GetAction(string pathName)
    {
      return catalog[pathName];
    }
  }
}
