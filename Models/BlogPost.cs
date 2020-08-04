using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Blog.Models
{
      public class BlogPost
      {
            public int Id { get; set; }
            public DateTimeOffset Created { get; set; }
            public DateTimeOffset? Updated { get; set; }
            public string Title { get; set; }
            public string Slug { get; set; }
            public string Abstract { get; set; }

            [AllowHtml]
            public string Body { get; set; }
            public string MediaPath { get; set; }
            public bool Published { get; set; }
            public virtual ICollection<Comment> Comments { get; set; }

            public BlogPost()
            {
                  Comments = new HashSet<Comment>();
            }
      }
}