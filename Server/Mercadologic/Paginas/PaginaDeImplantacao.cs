//using System;
//using System.Linq;
//using System.Threading.Tasks;
//using Mercadologic.Configuracoes;
//using Keep.Paper.Api;
//using Keep.Paper.Services;
//using Keep.Tools;
//using Microsoft.AspNetCore.Authorization;

//namespace Mercadologic.Paginas
//{
//  [Expose]
//  [AllowAnonymous]
//  public class PaginaDeImplantacao : AbstractPaper
//  {
//    public const string Titulo = "Configuração de Primeiro Uso";
//    private readonly IPaperCatalog paperCatalog;
//    private readonly ConfiguracoesDePrimeiroUso configuracoes;

//    public class Formulario
//    {
//      public string PaginaDestino { get; set; }

//      public string Servidor { get; set; }
//      public string BaseDeDados { get; set; }
//      public string Usuario { get; set; }
//      public string Senha { get; set; }
//    }

//    public PaginaDeImplantacao(IPaperCatalog paperCatalog, ICommonSettings settings)
//    {
//      this.paperCatalog = paperCatalog;
//      this.configuracoes = new ConfiguracoesDePrimeiroUso(settings);
//    }

//    public object Index(Formulario form)
//    {
//      form ??= new Formulario();

//      if (form.PaginaDestino == null)
//      {
//        var homePaper = paperCatalog.GetType(PaperName.Home);
//        form.PaginaDestino = Href.To(HttpContext, homePaper);
//      }

//      var campos = (
//        from propriedade in typeof(Formulario).GetProperties()
//        let isSenha = propriedade.Name == nameof(Formulario.Senha)
//        let isUsuario = propriedade.Name == nameof(Formulario.Usuario)
//        let isDestino = propriedade.Name == nameof(Formulario.PaginaDestino)
//        select new
//        {
//          Kind = isUsuario
//            ? FieldKind.Username
//            : isSenha
//              ? FieldKind.Password
//              : FieldKind.Text,
//          View = new
//          {
//            Name = propriedade.Name.ChangeCase(TextCase.CamelCase),
//            Hidden = isDestino
//          }
//        }
//      ).ToArray();

//      return new
//      {
//        Kind = Kind.Paper,
//        data = form,
//        View = new
//        {
//          Title = Titulo,
//          Design = Design.Action
//        },
//        fields = campos,
//        Links = new
//        {
//          Self = new
//          {
//            Title = "Autenticar",
//            Href = Href.To(HttpContext, GetType())
//          },
//          Action = new
//          {
//            Title = "Salvar Configurações",
//            Href = Href.To(HttpContext, GetType(), nameof(SalvarAsync))
//          },
//        }
//      };
//    }

//    public async Task<object> SalvarAsync(Formulario formulario)
//    {
//      var stringDeConexao =
//        $"Server={formulario.Senha};" +
//        $"Database={formulario.BaseDeDados};" +
//        $"User ID={formulario.Usuario};" +
//        $"Password={formulario.Senha};" +
//        $"Connect Timeout=180";

//      configuracoes.StringsDeConexao.Director = stringDeConexao;
//      configuracoes.Implantado = true;

//      return await Task.FromResult(new
//      {
//        Links = new object[]
//        {
//          new
//          {
//            Rel = Rel.Self,
//            Href = Href.To(HttpContext, GetType(), Name.Action())
//          },
//          new
//          {
//            Rel = Rel.Forward,
//            Href = formulario.PaginaDestino
//          }
//        }
//      });
//    }
//  }
//}
