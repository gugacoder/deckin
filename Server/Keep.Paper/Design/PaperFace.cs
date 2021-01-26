using System;
namespace Keep.Paper.Design
{
  public class PaperFace
  {
    private PaperFace()
    {
    }

    public string Name => GetType().Name;

    public class Form : PaperFace
    {
    }

    public class Grid : PaperFace
    {
    }
  }
}
