using System;
using System.Collections.Generic;
using Keep.Tools.Collections;
using Newtonsoft.Json;

namespace Keep.Paper.Api
{
  public static class Types
  {
    public interface IEntity
    {
      [JsonProperty(Order = 10)]
      string Kind { get; }

      [JsonProperty(Order = 20)]
      object Meta { get; }

      [JsonProperty(Order = 30)]
      object Data { get; }

      [JsonProperty(Order = 40)]
      View View { get; }

      [JsonProperty(Order = 50)]
      Collection<Field> Fields { get; }

      [JsonProperty(Order = 60)]
      Collection<Action> Actions { get; }

      [JsonProperty(Order = 70)]
      Collection<Entity> Embedded { get; }

      [JsonProperty(Order = 80)]
      Collection<Link> Links { get; }
    }

    public class Entity : IEntity
    {
      [JsonProperty(Order = 10)]
      public string Kind { get; set; }

      [JsonProperty(Order = 20)]
      public object Meta { get; set; }

      [JsonProperty(Order = 30)]
      public object Data { get; set; }

      [JsonProperty(Order = 40)]
      public View View { get; set; }

      [JsonProperty(Order = 50)]
      public Collection<Field> Fields { get; set; }

      [JsonProperty(Order = 60)]
      public Collection<Action> Actions { get; set; }

      [JsonProperty(Order = 70)]
      public Collection<Entity> Embedded { get; set; }

      [JsonProperty(Order = 80)]
      public Collection<Link> Links { get; set; }
    }

    public class Field : IEntity
    {
      [JsonProperty(Order = 10)]
      public string Kind { get; set; }

      [JsonProperty(Order = 20)]
      public object Meta { get; set; }

      [JsonProperty(Order = 30)]
      public object Data { get; set; }

      [JsonProperty(Order = 40)]
      public View View { get; set; }

      [JsonProperty(Order = 50)]
      Collection<Field> IEntity.Fields { get; }

      [JsonProperty(Order = 60)]
      Collection<Action> IEntity.Actions { get; }

      [JsonProperty(Order = 70)]
      Collection<Entity> IEntity.Embedded { get; }

      [JsonProperty(Order = 80)]
      Collection<Link> IEntity.Links { get; }
    }

    public class Action : IEntity
    {
      [JsonProperty(Order = 10)]
      public string Kind { get; set; }

      [JsonProperty(Order = 20)]
      public object Meta { get; set; }

      [JsonProperty(Order = 30)]
      public object Data { get; set; }

      [JsonProperty(Order = 40)]
      public View View { get; set; }

      [JsonProperty(Order = 50)]
      public Collection<Field> Fields { get; set; }

      [JsonProperty(Order = 60)]
      Collection<Action> IEntity.Actions { get; }

      [JsonProperty(Order = 70)]
      Collection<Entity> IEntity.Embedded { get; }

      [JsonProperty(Order = 80)]
      public Collection<Link> Links { get; set; }
    }

    public class View
    {
      [JsonProperty(Order = 10)]
      public string Name { get; set; }

      [JsonProperty(Order = 20)]
      public string Title { get; set; }

      [JsonProperty(Order = 30)]
      public IDesign Design { get; set; }
    }

    public class Link
    {
      [JsonProperty(Order = 10)]
      public string Rel { get; set; }

      [JsonProperty(Order = 20)]
      public string Title { get; set; }

      [JsonProperty(Order = 30)]
      public object Data { get; set; }

      [JsonProperty(Order = 40)]
      public string Href { get; set; }
    }

    public interface IDesign
    {
      [JsonProperty(Order = 10)]
      string Kind { get; }
    }

    public class GridDesign : IDesign
    {
      [JsonProperty(Order = 10)]
      public string Kind => Design.Grid;

      /// <summary>
      /// Intervalo de autoatualização de dados em segundos.
      /// </summary>
      [JsonProperty(Order = 20)]
      public int AutoRefresh { get; set; }

      [JsonProperty(Order = 30)]
      public Pagination Page { get; set; }
    }
  }
}
