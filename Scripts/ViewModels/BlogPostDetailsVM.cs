using Blog.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Blog.Scripts.ViewModels
{
      public class BlogPostDetailsVM
      {
            public BlogPost BlogPost { get; set; }
            public ICollection<BlogPost> SidePost { get; set; }
            public BlogPostDetailsVM()
            {
                 //SidePosts = new HashSet<BlogPost>();
            }
      }
}