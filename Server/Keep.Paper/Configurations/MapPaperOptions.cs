using System;
using Keep.Paper.Api;
using Keep.Paper.Papers;

namespace Keep.Paper.Configurations
{
  public class MapPaperOptions
  {
    internal Type HomePaper { get; private set; }
    internal Type LoginPaper { get; private set; }

    public MapPaperOptions AddHomePaper<T>()
      where T : IPaper
    {
      return AddHomePaper(typeof(T));
    }

    public MapPaperOptions AddHomePaper(Type paperType)
    {
      this.HomePaper = paperType;
      return this;
    }

    public MapPaperOptions AddLoginPaper<T>()
      where T : IPaper
    {
      return AddLoginPaper(typeof(T));
    }

    public MapPaperOptions AddLoginPaper(Type paperType)
    {
      this.LoginPaper = paperType;
      return this;
    }
  }
}
