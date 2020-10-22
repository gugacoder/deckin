using System;
using System.Linq;

namespace Keep.Paper.Types
{
  public class Credential
  {
    private string _username;

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
  }
}
