using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Xml.Linq;
using System.Xml.Serialization;
using Keep.Tools;
using Keep.Tools.Collections;
using Newtonsoft.Json;

namespace Keep.Paper.Api.Types
{
  [Serializable]
  public abstract class Action : Types.IEntity
  {
    private Collection<Link> _links;
    private Link _target;

    protected virtual string ProtectedKind { get; set; } = Api.Kind.Action;
    protected virtual string ProtectedDesign { get; set; }
    protected virtual object ProtectedData { get; set; }

    [JsonProperty(Order = -1090)]
    public virtual string Kind => ProtectedKind;

    [JsonProperty(Order = -1080)]
    public virtual string Name { get; set; }

    [JsonProperty(Order = -1070)]
    public virtual string Rel { get; set; }

    [JsonProperty(Order = -1060)]
    public virtual object Meta { get; set; }

    [JsonProperty(Order = -1050)]
    public virtual string Design => ProtectedDesign;

    [JsonProperty(Order = -90)]
    public virtual string Title { get; set; }

    [JsonProperty(Order = -80)]
    public virtual string Extent { get; set; }

    [JsonProperty(Order = 1000)]
    public virtual Link Target
    {
      get => Links?.FirstOrDefault(x => Api.Rel.Action.EqualsIgnoreCase(x.Rel));
      set
      {
        if (Links != null)
        {
          Links.RemoveWhen(x => Api.Rel.Action.EqualsIgnoreCase(x.Rel));
        }

        _target = value;

        if (value != null)
        {
          value.Rel = Api.Rel.Action;
          (Links ??= new Collection<Link>()).Add(value);
        }
      }
    }

    [JsonProperty(Order = 1010)]
    public virtual string DataType { get; set; }

    [JsonProperty(Order = 1020)]
    public virtual object Data
    {
      get => ProtectedData;
      set => ProtectedData = value;
    }

    [XmlArray]
    [JsonProperty(Order = 1010)]
    public virtual Collection<Types.Field> Fields { get; set; }

    [XmlArray]
    [JsonProperty(Order = 1020)]
    public virtual Collection<Types.Action> Actions { get; set; }

    [XmlArray]
    [JsonProperty(Order = 1030)]
    public virtual Collection<Types.IEntity> Embedded { get; set; }

    [XmlArray]
    [JsonProperty(Order = 1040)]
    public virtual Collection<Types.Link> Links
    {
      get => _links;
      set
      {
        _links = value;

        if (_target != null)
        {
          _links ??= new Collection<Link>();

          var hasTarget = _links.Any(x => Api.Rel.Action.EqualsIgnoreCase(x.Rel));
          if (hasTarget)
          {
            _target = null;
          }
          else
          {
            _links.Add(_target);
          }
        }
      }
    }
  }
}