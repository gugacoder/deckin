using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Keep.Paper.Catalog;
using Keep.Tools;

namespace Keep.Paper.Templating
{
  [Expose]
  public class TemplateActionFactory : IActionFactory
  {
    public async Task<ICollection<IAction>> CreateActionsAsync()
    {
      return await Task.FromResult(new IAction[0]);
    }
  }
}
