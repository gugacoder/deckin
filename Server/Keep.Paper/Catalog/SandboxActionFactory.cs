using System;
using System.Threading.Tasks;
using Keep.Paper.Api;
using Types = Keep.Paper.Api.Types;
using Keep.Paper.Rendering;
using Keep.Tools;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using System.Reflection;
using System.Linq;
using Keep.Tools.Collections;
using Keep.Tools.Reflection;

namespace Keep.Paper.Catalog
{
  public class DB : List<DB.IData>
  {
    public interface IData
    {
      int? Id { get; set; }
    }

    public class Item : IData
    {
      public int? Id { get; set; }
      public string Name { get; set; }
      public decimal Price { get; set; }
    }

    public ICollection<T> Search<T>(Func<IData, bool> predicate = null)
      where T : IData
    {
      lock (this)
      {
        return this
        .OfType<T>()
        .Where(x => predicate == null || predicate.Invoke(x))
        .ToArray();
      }
    }

    public T Create<T>(T data)
      where T : IData
    {
      lock (this)
      {
        if (data.Id == null)
        {
          var maxId = this.OfType<T>().DefaultIfEmpty().Max(x => x?.Id) ?? 0;
          data.Id = maxId + 1;
        }

        var exists = this.OfType<T>().Any(x => x.Id == data.Id);
        if (exists)
          throw new HttpException(StatusCodes.Status409Conflict,
            $"A entidade já existe: {data.Id}");

        this.Add(data);

        return data;
      }
    }

    public T Read<T>(int id)
      where T : IData
    {
      lock (this)
      {
        return this.OfType<T>().FirstOrDefault(x => x.Id == id);
      }
    }

    public T Update<T>(T data)
      where T : IData
    {
      lock (this)
      {
        this.RemoveAll(x => x.Id == data.Id);
        this.Add(data);
        return data;
      }
    }

    public T Delete<T>(int id)
      where T : IData
    {
      lock (this)
      {
        var current = this.OfType<T>().FirstOrDefault(x => x.Id == id);
        this.RemoveAll(x => x.Id == id);
        return current;
      }
    }
  }

  [Expose]
  [Collection("Items")]
  public class SandboxActionFactory : IActionFactory
  {
    private readonly static DB DB = new DB();

    public async Task<ICollection<IAction>> CreateActionsAsync()
    {
      var flags = BindingFlags.DeclaredOnly
                | BindingFlags.Instance
                | BindingFlags.Public;

      var methods = (
        from m in GetType().GetMethods(flags)
        where !m.Name.Equals(nameof(CreateActionsAsync))
        select m.Name
        ).ToArray();

      var actions = methods.ToArray(name => MethodAction.Create(this, name));
      return await Task.FromResult(actions);
    }

    public Types.Entity Search(HashMap<string> criteria, Pagination page)
    {
      criteria ??= new HashMap<string>();

      if (page?.Offset == null && page?.Limit == null)
      {
        (page ??= new Pagination()).Limit = 50;
      }

      var records = DB.Search<DB.Item>(record => criteria.Keys.All(key =>
      {
        var prop = record._Define(key) as PropertyInfo;
        if (prop == null)
          return false;

        var value = criteria[key];
        var castValue = Change.To(value, prop.PropertyType);

        var currentValue = prop.GetValue(record);

        return Equals(currentValue, castValue);
      })).AsEnumerable();

      if (page.Offset != null)
      {
        records = records.Skip(page.Offset.Value);
      }

      if (page.Limit != null)
      {
        records = records.Take(page.Limit.Value);
      }

      return new Types.Action
      {
        Props = new Types.GridView
        {
        },
        Actions = new Types.ActionCollection
        {
          new Types.Action
          {
            Props = new Types.FormView
            {
              Name = "Filter"
            },
            Fields = new Types.FieldCollection
            {
              new Types.Field
              {
                Props = new Types.IntWidget
                {
                  Name = nameof(DB.Item.Id)
                }
              },
              new Types.Field
              {
                Props = new Types.TextWidget
                {
                  Name = nameof(DB.Item.Name)
                }
              },
              new Types.Field
              {
                Props = new Types.DecimalWidget
                {
                  Name = nameof(DB.Item.Price)
                }
              }
            }
          },
          new Types.Action
          {
            Props = new Types.FormView
            {
              Name = nameof(DB.Create),
              Target = new Types.Link
              {
                 Rel = Rel.Action,
                 Href =  $"/Api/1/Papers/Items.Create"
              }
            },
            Fields = new Types.FieldCollection
            {
              new Types.Field
              {
                Props = new Types.TextWidget
                {
                  Name = nameof(DB.Item.Name)
                }
              },
              new Types.Field
              {
                Props = new Types.DecimalWidget
                {
                  Name = nameof(DB.Item.Price)
                }
              }
            }
          }
        },
        Embedded = new Types.EntityCollection(records.Select(record =>
          new Types.Action
          {
            Href = $"/Api/1/Papers/Items.Read({record.Id})"
          }
        )),
        Links = new Types.LinkCollection
        {
          new Types.Link
          {
             Rel = Rel.Self,
             Href =  $"/Api/1/Papers/Items.Search"
          },
        }
      };
    }

    public Types.Entity Read(int id)
    {
      var record = DB.Read<DB.Item>(id);
      if (record == null)
        throw new HttpException(StatusCodes.Status404NotFound);

      return new Types.Action
      {
        Data = new Types.Data(record),
        Props = new Types.CardView
        {
        },
        Actions = new Types.ActionCollection
        {
          new Types.Action
          {
            Props = new Types.FormView
            {
              Name = nameof(DB.Update),
              Target = new Types.Link
              {
                 Rel = Rel.Action,
                 Href =  $"/Api/1/Papers/Items.Update({id})"
              }
            },
            Fields = new Types.FieldCollection
            {
              new Types.Field
              {
                Props = new Types.TextWidget
                {
                  Name = nameof(DB.Item.Name)
                }
              },
              new Types.Field
              {
                Props = new Types.DecimalWidget
                {
                  Name = nameof(DB.Item.Price)
                }
              }
            }
          },
          new Types.Action
          {
            Props = new Types.FormView
            {
              Name = nameof(DB.Delete),
              Target = new Types.Link
              {
                 Rel = Rel.Action,
                 Href =  $"/Api/1/Papers/Items.Delete({id})"
              }
            }
          }
        },
        Links = new Types.LinkCollection
        {
          new Types.Link
          {
             Rel = Rel.Self,
             Href =  $"/Api/1/Papers/Items.Read({id})"
          },
          new Types.Link
          {
             Rel = Rel.Link,
             Href =  $"/Api/1/Papers/Items.Search"
          }
        },
      };
    }

    public Types.Entity Create(DB.Item item)
    {
      item = DB.Create(item);
      return Read(item.Id.Value);
    }

    public Types.Entity Update(int id, DB.Item item)
    {
      item = DB.Update(item);
      return Read(item.Id.Value);
    }

    public Types.Entity Delete(int id)
    {
      DB.Delete<DB.Item>(id);
      return Search(null, null);
    }

    [Action("Echo(...)")]
    public async Task<object> EchoAsync(IPath path, IPathArgs args)
    {
      return await Task.FromResult(new Types.Status
      {
        Data = new Types.Data(new
        {
          path,
          args
        }),
        Props = new Types.Status.Info
        {
          Reason = "Ok",
          Severity = Severity.Success
        }
      });
    }
  }
}
