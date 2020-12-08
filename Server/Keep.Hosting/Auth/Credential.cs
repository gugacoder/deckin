using System;
using System.Linq;

namespace Keep.Hosting.Auth
{
  public class Credential
  {
    private string _username;

    public Credential()
    {
    }

    public Credential(string username, string password, string domain = null)
    {
      this.Domain = domain;
      this.Username = username;
      this.Password = password;
    }

    public string Domain
    {
      get;
      set;
    }

    public string Username
    {
      get => _username;
      set
      {
        if (value?.Contains("/") == true)
        {
          var tokens = value.Split('/');
          Domain = tokens.First();
          _username = tokens.Last();
        }
        else
        {
          _username = value;
        }
      }
    }

    public string Password
    {
      get;
      set;
    }

    public override string ToString()
    {
      return (Domain != null) ? $"{Domain}/{Username}" : Username;
    }
  }
}
