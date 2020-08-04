namespace Blog.Migrations
{
      using Blog.Models;
      using Microsoft.AspNet.Identity;
      using Microsoft.AspNet.Identity.EntityFramework;
      using System;
      using System.CodeDom;
      using System.Data.Entity;
      using System.Data.Entity.Migrations;
      using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<Blog.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(Blog.Models.ApplicationDbContext context)
        {
                  var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));

                  if (!context.Roles.Any(r => r.Name == "Admin"))
                  {
                        roleManager.Create(new IdentityRole { Name = "Admin" });
                  }

                  if (!context.Roles.Any(r => r.Name == "Moderator"))
                  {
                        roleManager.Create(new IdentityRole { Name = "Moderator" });
                  }

                  var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

                  if (!context.Users.Any(u => u.Email == "RichardKnott1970@gmail.com"))
                  {
                        userManager.Create(new ApplicationUser()
                        {
                              Email = "RichardKnott1970@gmail.com",
                              UserName = "RichardKnott1970@gmail.com",
                              FirstName = "Richard",
                              LastName = "Knott",
                              DisplayName = "Richard"
                        }, "!R464121k@");

                        //Get the ID that was just created by adding the above user
                        var userId = userManager.FindByEmail("Richardknott1970@gmail.com").Id;
                        //assign userId to a role
                        userManager.AddToRole(userId, "Admin");
                  }

                  if (!context.Users.Any(u => u.Email == "JasonTwichell@CoderFoundry.com"))
                  {
                        userManager.Create(new ApplicationUser()
                        {
                              Email = "JasonTwichell@CoderFoundry.com",
                              UserName = "JasonTwichell@CoderFoundry.com",
                              FirstName = "Jason",
                              LastName = "Twichell",
                              DisplayName = "Jason"
                        }, "Abc&123!");

                        //Get the ID that was just created by adding the above user
                        var userId = userManager.FindByEmail("JasonTwichell@CoderFoundry.com").Id;
                        //assign userId to a role
                        userManager.AddToRole(userId, "Moderator");
                  }

                  if (!context.Users.Any(u => u.Email == "arussell@coderfoundry.com"))
                  {
                        userManager.Create(new ApplicationUser()
                        {
                              Email = "arussell@coderfoundry.com",
                              UserName = "arussell@coderfoundry.com",
                              FirstName = "Drew",
                              LastName = "Russell",
                              DisplayName = "Drew"
                        }, "!Abc&123");

                        //Get the ID that was just created by adding the above user
                        var userId = userManager.FindByEmail("arussell@CoderFoundry.com").Id;
                        //assign userId to a role
                        userManager.AddToRole(userId, "Moderator");
                  }
            }
    }
}
