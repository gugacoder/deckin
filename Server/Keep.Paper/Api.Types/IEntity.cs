using System;
using System.Collections.Generic;
using Keep.Tools.Collections;
using Newtonsoft.Json;

namespace Keep.Paper.Api.Types
{
  public interface IEntity
  {
    string Kind { get; }

    object Meta { get; }

    object Data { get; }

    View View { get; }

    Collection<Field> Fields { get; }

    Collection<Action> Actions { get; }

    Collection<Entity> Embedded { get; }

    Collection<Link> Links { get; }
  }
}
