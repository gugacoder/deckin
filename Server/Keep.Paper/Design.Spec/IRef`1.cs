using System;
namespace Keep.Paper.Design.Spec
{
  public interface IRef<T> : IRef
    where T : IEntity
  {
  }
}
