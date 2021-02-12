using System;
namespace Keep.Paper.Design.Spec
{
  public interface IEntityRef : IDesign
  {
    string Type { get; set; }

    Ref Ref { get; set; }
  }
}
