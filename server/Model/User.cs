using System;
using System.Collections.Generic;
using System.Linq;

namespace Server.Model
{
  public class User
  {
    public int Id { get; set; }

    public string Name { get; set; }

    public static User Retrieve(int id)
    {
      return FIXTURE.FirstOrDefault(x => x.Id == id);
    }

    public static void Index() { }

    public static int ParseKey(string key)
    {
      return int.Parse(key);
    }

    public static List<User> FIXTURE = new List<User>
    {
      new User
      {
        Id = 1,
        Name = "First"
      },
      new User
      {
        Id = 2,
        Name = "Second"
      }
    };
  }
}
