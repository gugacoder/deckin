using System;
using System.Collections.Generic;
using Keep.Tools.Collections;
using Newtonsoft.Json;

namespace Keep.Paper.Api.Types
{
  public interface IEntity
  {
    string Kind { get; }

    string Name { get; }

    string Rel { get; }

    object Meta { get; }

    string DataType { get; }

    object Data { get; }

    Collection<Types.Field> Fields { get; }

    Collection<Types.Action> Actions { get; }

    Collection<Types.IEntity> Embedded { get; }

    Collection<Types.Link> Links { get; }
  }
}
