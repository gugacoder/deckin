using System;
namespace Keep.Paper.Api.Types
{
  public interface IFieldEntity : IEntity
  {
    string Title { get; }

    bool? Hidden { get; }

    bool? Required { get; }

    string Extent { get; }

    Options Options { get; }
  }
}
