using System;
using Keep.Tools.Collections;
using Newtonsoft.Json;

namespace Keep.Paper.Api.Types
{
  public class Status : Types.IEntity
  {
    protected virtual string ProtectedKind { get; set; } = Api.Kind.Status;
    protected virtual string ProtectedDesign { get; set; }

    [JsonProperty(Order = -1090)]
    public virtual string Kind => ProtectedKind;

    [JsonProperty(Order = 10)]
    public virtual string Text { get; set; }

    [JsonProperty(Order = 10)]
    public virtual string Detail { get; set; }

    [JsonProperty(Order = 20)]
    public virtual string Severity { get; set; }

    [JsonProperty(Order = 30)]
    public virtual string Field { get; set; }

    [JsonProperty(Order = 40)]
    public virtual string StackTrace { get; set; }

    string Types.IEntity.Name { get; }
    object Types.IEntity.Meta { get; }
    object Types.IEntity.Data { get; }
    Collection<Types.Field> IEntity.Fields { get; }
    Collection<Types.Action> IEntity.Actions { get; }
    Collection<Types.Entity> IEntity.Embedded { get; }
    Collection<Types.Link> IEntity.Links { get; }
  }
}
