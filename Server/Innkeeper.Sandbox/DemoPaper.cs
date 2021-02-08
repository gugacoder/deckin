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
    public Response<Paper> GetPaper(Request request)
      => Response.For(new Paper
      {
        DataSet = new DataSet
        {
          Data = new Collection<IRef<Data>>(
            from id in Enumerable.Range(1, 10)
            select new Data
            {
              Properties = new
              {
                Id = id,
                Name = $"Data ID {id}"
              }
            }
          )
        },
        Disposition = new Disposition.Grid()
      });

    public Response<Data> GetData(Request request, int id)
      => Response.For(new Data
      {
        Properties = new
        {
          Id = id,
          Name = $"Data ID {id}"
        }
      });

    public Response<DataSet> GetDataSet(Request request)
      => Response.For(new DataSet
      {
        Data = new Collection<IRef<Data>>(
          from id in Enumerable.Range(1, 10)
          select new Data
          {
            Properties = new
            {
              Id = id,
              Name = $"Data ID {id}"
            }
          }
        )
      });

    public Response<Disposition.Card> GetCard(Request request)
      => Response.For(new Disposition.Card());

    public Response<Disposition.Edit> GetEdit(Request request)
      => Response.For(new Disposition.Edit());

    public Response<Disposition.List> GetList(Request request)
      => Response.For(new Disposition.List());

    public Response<Disposition.Grid> GetGrid(Request request)
      => Response.For(new Disposition.Grid());
  }
}
