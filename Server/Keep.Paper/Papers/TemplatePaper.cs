using System;
using Keep.Paper.Api;
using Keep.Paper.Api.Types;

namespace Keep.Paper.Papers
{
  public class TemplatePaper : AbstractPaper
  {
    private readonly Template template;

    public TemplatePaper(Template template)
    {
      this.template = template;
    }

    public object Index() => new
    {
      Kind = Kind.Paper,
      Data = template
    };
  }
}
