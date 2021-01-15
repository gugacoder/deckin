using System;
using Keep.Tools.Collections;

namespace Keep.Paper.Design
{
  public class Action : Entity
  {
    public Collection<Job> Jobs { get; set; }
    public Collection<Notification> Notifications { get; set; }
  }
}
