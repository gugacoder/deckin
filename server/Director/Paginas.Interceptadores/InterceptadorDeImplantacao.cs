//using System;
//using System.Threading.Tasks;
//using Keep.Paper.Api;
//using Keep.Paper.Services;
//using Keep.Tools;
//using Microsoft.AspNetCore.Http.Extensions;

//namespace Director.Paginas.Interceptadores
//{
//  [Expose]
//  public class InterceptadorDeImplantacao : AbstractPaperInterceptor
//  {
//    public InterceptadorDeImplantacao(ICommonSettings settings)
//    {
//    }

//    public override async Task<object> InterceptPaper(PaperInfo info, NextAsync nextAsync)
//    {
//      var ignorado = info.Paper is PaginaDeImplantacao;
//      var implantado = configuracoes.Implantado;

//      if (!ignorado && !implantado)
//      {
//        var resultado = DirecionarParaImplantacao(info);
//        return await Task.FromResult(resultado);
//      }

//      return await nextAsync(info);
//    }

//    private object DirecionarParaImplantacao(PaperInfo info)
//    {
//      var ctx = this.HttpContext;
//      return new
//      {
//        Kind = Kind.Fault,
//        Data = new
//        {
//          Fault = Fault.Forbidden,
//          Reason = new[]{
//            "O sistema ainda não está configurado para uso."
//          }
//        },
//        Links = new object[]
//        {
//          new
//          {
//            Rel = Rel.Self,
//            Href = HttpContext.Request.GetDisplayUrl()
//          },
//          new
//          {
//            PaginaDeImplantacao.Titulo,
//            Rel = Rel.Forward,
//            Href = Href.To(ctx,
//              typeof(PaginaDeImplantacao),
//              nameof(PaginaDeImplantacao.Index)),
//            Data = new {
//              Form = new {
//                PaginaDestino = Href.To(HttpContext, info)
//              }
//            }
//          }
//        }
//      };
//    }
//  }
//}
