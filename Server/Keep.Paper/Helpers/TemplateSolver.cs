//using System;
//using Keep.Paper.Api;
//using Keep.Paper.Api.Types;
//using Keep.Paper.Papers;
//using Microsoft.Extensions.DependencyInjection;

//namespace Keep.Paper.Helpers
//{
//  public class TemplateSolver
//  {
//    public TemplateSolver(IAudit audit)
//    {
//    }

//    public PaperType[] CreatePaperTypes(Template template)
//    {
//      return new PaperType[]
//      {
//        new PaperType
//        {
//          Catalog = template.Catalog,
//          Name = template.Name,
//          Type = typeof(TemplatePaper),
//          Factory = provider => CreateInstance(provider, template)
//        }
//      };
//    }

//    private IPaper CreateInstance(IServiceProvider provider, Template template)
//    {
//      return ActivatorUtilities.CreateInstance<TemplatePaper>(provider, template);
//    }
//  }
//}
