using System;
using Keep.Paper.Design;
using Keep.Paper.Design.Modeling;
using Keep.Tools;
using Keep.Tools.Collections;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;

namespace Keep.Paper.Papers
{
  [Expose]
  public class LoginPaper : IPaper
  {
    public IDesign Login(Form form)
    {
      var jObject = form.Properties as JObject;
      var username = jObject?["username"]?.ToString();
      var password = jObject?["password"]?.ToString();
      var redirect = jObject?["redirect"]?.ToString();
      return new Design.Paper
      {
        Data = new Collection<Data>
        {
          new Data
          {
            Properties = new
            {
              Username = username,
              Password = password,
              Redirect = redirect,
            }
          }
        },
        View = new View
        {
          Target = Ref.Create(GetType(), nameof(Authenticate)),
          Face = new PaperFace.Form(),
          Fields = new Collection<Field>
          {
            new Field
            {
              Name = "Username",
              Type = TypeNames.Text,
              Face = new FieldFace.Input(),
              Title = "Usuário"
            },
            new Field
            {
              Name = "Password",
              Type = TypeNames.Text,
              Face = new FieldFace.Input(),
              Title = "Senha",
              Password = true
            }
          }
        }
      };
    }

    public IDesign Authenticate(Form form)
    {
      var jObject = form.Properties as JObject;
      var username = jObject?["username"]?.ToString();
      var password = jObject?["password"]?.ToString();
      var redirect = jObject?["redirect"]?.ToString();
      if (username == "admin" && password == "admin")
      {
        return new Status
        {
          Code = StatusCodes.Status302Found,
          Location = redirect ?? "Home",
        };
      }
      else
      {
        return new Validation
        {
          Status = new Collection<Status>
          {
            new Status
            {
              Code = StatusCodes.Status401Unauthorized,
              Message = "Usuário e senha não conferem.",
              Field = "Username"
            }
          }
        };
      }
    }
  }
}