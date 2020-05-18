using System;
using System.Collections.Generic;
using System.Linq;

namespace Innkeeper.Papers
{
  public class UserPaper
  {
    public int Id { get; set; }

    public string Name { get; set; }

    public static UserPaper Retrieve(int id)
    {
      return FIXTURE.FirstOrDefault(x => x.Id == id);
    }

    public static ICollection<UserPaper> Index(object filter)
    {
      return FIXTURE;
    }

    public static object GetKey(UserPaper user)
    {
      return user.Id;
    }

    public static int ParseKey(string key)
    {
      return int.Parse(key);
    }

    public static List<UserPaper> FIXTURE = new List<UserPaper>
    {
      new UserPaper
      {
        Id = 1,
        Name = "First"
      },
      new UserPaper
      {
        Id = 2,
        Name = "Second"
      }
    };
  }
}
