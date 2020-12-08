﻿using System;
namespace Keep.Hosting.Api
{
  [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Method)]
  public class PaperNameAttribute : Attribute
  {
    public PaperNameAttribute()
    {
    }

    public PaperNameAttribute(string paperName)
    {
      this.PaperName = paperName;
    }

    public string PaperName { get; set; }
  }
}
