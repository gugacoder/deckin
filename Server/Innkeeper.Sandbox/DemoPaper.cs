using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Keep.Paper.Design;
using Keep.Paper.Design.Modeling;
using Keep.Paper.Design.Rendering;
using Keep.Paper.Design.Serialization;
using Keep.Tools;
using Keep.Tools.Collections;
using Keep.Tools.Reflection;
using Keep.Paper.Design.Spec;

namespace Innkeeper.Sandbox
{
  [Expose]
  public class DemoPaper : IPaperDesign
  {
    public Response<Perspective> GetPerspective(Request request, int id)
      => Response.For(new Perspective());

    public Response<Data> GetData(Request request, int id)
      => Response.For(new Data
      {
        Links = new Collection<Link>
        {
          new Link
          {
            Rel = RelNames.Perspective,
            Ref = Ref.For(typeof(DemoPaper), nameof(GetPerspective))
          }
        },
        Properties = new
        {
          Id = id,
          Name = $"Data ID {id}"
        },
        Subset = new Collection<IEntityRef<Data>>
        {
          EntityRef.For<Data>("User", new { Id = 105, Login = "Tananana" }),
          new Data
          {
            Self = Ref.For("User", new { Id = 105, Login = "Tananana" }),
            Properties = new
            {
              Id = 105,
              Login = "Tananana",
              Name = "Fulano de Talz"
            }
          }
        }
      });

    public Response<Data> GetDataSet(Request request)
      => Response.For(new Data
      {
        Self = "Data/DemoPaper.GetDataSet",
        Subset = new Collection<IEntityRef<Data>>(
          from id in Enumerable.Range(1, 10)
          select new Data
          {
            Self = $"Data/DemoPaper.GetData(id={id})",
            Properties = new
            {
              Id = id,
              Name = $"Data ID {id}"
            },
            Subset = new Collection<IEntityRef<Data>>(
              from idx in Enumerable.Range(100, 3)
              select new Data
              {
                Properties = new
                {
                  Id = idx,
                  Name = $"Data ID {idx}"
                }
              }
            )
          }
        )
      });

    public Response<Paper> GetPaper(Request request)
      => Response.For(new Paper
      {
        DataSet = new DataSet
        {
          Subset = GetDataSet(request).Entity.Subset
        },
        Disposition = new Blueprint.Grid()
      });

    public Response<Blueprint.Card> GetCard(Request request)
      => Response.For(new Blueprint.Card());

    public Response<Blueprint.Edit> GetEdit(Request request)
      => Response.For(new Blueprint.Edit());

    public Response<Blueprint.List> GetList(Request request)
      => Response.For(new Blueprint.List());

    public Response<Blueprint.Grid> GetGrid(Request request)
      => Response.For(new Blueprint.Grid());
  }
}
