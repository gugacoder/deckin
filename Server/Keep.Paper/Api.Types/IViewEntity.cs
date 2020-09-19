using System;
namespace Keep.Paper.Api.Types
{
  public interface IViewEntity : IEntity
  {
    string Title { get; set; }
  }
}
