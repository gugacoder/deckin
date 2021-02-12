using System;
namespace Keep.Paper.Design.Spec
{
  public interface IEntityRef<T> : IEntityRef, IDesign
    where T : class, IEntity
  {
  }
}
