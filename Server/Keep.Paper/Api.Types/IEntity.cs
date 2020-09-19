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

    object Meta { get; }

    string Design { get; }

    object Data { get; }

    Collection<Field> Fields { get; }

    Collection<Action> Actions { get; }

    Collection<Entity> Embedded { get; }

    Collection<Link> Links { get; }
  }
}
