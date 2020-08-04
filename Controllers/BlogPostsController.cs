using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using Blog.Helpers;
using Blog.Models;
using PagedList;
using PagedList.Mvc;

namespace Blog.Controllers
{
      [RequireHttps]
      public class BlogPostsController : Controller
      {
            private ApplicationDbContext db = new ApplicationDbContext();

            // GET: BlogPosts
            public ActionResult Index(int? page, string searchStr)
            {
                  ViewBag.Search = searchStr;
                  var blogList = IndexSearch(searchStr);
                  int pageSize = 4; //specifies the number of posts per page
                  int pageNumber = (page ?? 1); //?? null coalesing operator
                  var model = blogList.ToPagedList(pageNumber, pageSize);
                  return View(model);
            }

            public IQueryable<BlogPost> IndexSearch(string searchStr)
            {
                  IQueryable<BlogPost> result = null;
                  if (searchStr != null)
                  {
                        result = db.Posts.AsQueryable();
                        result = result.Where(p => p.Title.Contains(searchStr) || 
                                                           p.Body.Contains(searchStr) || 
                                                           p.Comments.Any(c => c.Body.Contains(searchStr) || 
                                                           c.Author.FirstName.Contains(searchStr) || 
                                                           c.Author.LastName.Contains(searchStr) || 
                                                           c.Author.DisplayName.Contains(searchStr) || 
                                                           c.Author.Email.Contains(searchStr)));
                  }
                  else
                  {
                        result = db.Posts.AsQueryable();
                  }
                  return result.OrderByDescending(p => p.Created);
            }
          
            public ActionResult Details(string slug)
            {
                  if (String.IsNullOrWhiteSpace(slug))
                  {
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                  }

                  var blogPost = db.Posts.FirstOrDefault(p => p.Slug == slug);
                  if (blogPost == null)
                  {
                        return HttpNotFound();
                  }
                  return View(blogPost);
            }


            // GET: BlogPosts/Create
            [Authorize(Roles = "Admin")]
            public ActionResult Create()
            {
                  var placeHolder = new BlogPost();
                  return View(placeHolder);
            }

            // POST: BlogPosts/Create
            // To protect from overposting attacks, enable the specific properties you want to bind to, for 
            // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
            [HttpPost]
            [ValidateAntiForgeryToken]
            [Authorize(Roles = "Admin")]
            public ActionResult Create([Bind(Include = "Id,Created,Updated,Title,Slug,Body,Abstract, MediaPath,Published")] BlogPost blogPost, HttpPostedFileBase image)
            {
                  if (ModelState.IsValid)
                  {
                        blogPost.Created = DateTime.Now;
                        var slug = StringUtilities.URLFriendly(blogPost.Title);                        

                        //check to see if the slug is empty
                        if (String.IsNullOrWhiteSpace(slug))
                        {
                              ModelState.AddModelError("Title", "Invalid entry (cannot be blank).");
                              return View(blogPost);
                        }

                        //determine whether or not the slug has already been used
                        if (db.Posts.Any(p => p.Title == slug))
                        {
                              ModelState.AddModelError("Title", "Invalid entry (possible duplicate).");
                              return View(blogPost);
                        }
                        if (ImageUploadValidator.IsWebFriendlyImage(image))
                        {
                              var fileName = Path.GetFileName(image.FileName);
                              var justFileName = Path.GetFileNameWithoutExtension(fileName);
                              fileName = $"{justFileName}_{DateTime.Now.Ticks}{Path.GetExtension(fileName)}";
                              image.SaveAs(Path.Combine(Server.MapPath("~/Uploads/" + fileName)));
                              blogPost.MediaPath = "/Uploads/" + fileName;
                        }

                        blogPost.Slug = slug;
                        db.Posts.Add(blogPost);
                        db.SaveChanges();
                        return RedirectToAction("Index");
                  }

                  return View(blogPost);
            }

            // GET: BlogPosts/Edit/5
            [Authorize(Roles = "Admin, Moderator")]
            public ActionResult Edit(int? id)
            {
                  if (id == null)
                  {
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                  }
                  BlogPost blogPost = db.Posts.Find(id);
                  if (blogPost == null)
                  {
                        return HttpNotFound();
                  }
                  return View(blogPost);
            }

            // POST: BlogPosts/Edit/5
            // To protect from overposting attacks, enable the specific properties you want to bind to, for 
            // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
            [Authorize(Roles = "Admin, Moderator")]
            [HttpPost]
            [ValidateAntiForgeryToken]
            public ActionResult Edit([Bind(Include = "Id,Created,Updated,Title,Slug, Body,Abstract,MediaPath,Published")] BlogPost blogPost, HttpPostedFileBase image)
            {
                  if (ModelState.IsValid)
                  {
                        blogPost.Updated = DateTime.Now;
                        var slug = StringUtilities.URLFriendly(blogPost.Title);

                        //check to see if the slug is empty
                        if (String.IsNullOrWhiteSpace(slug))
                        {
                              ModelState.AddModelError("Title", "Invalid entry (cannot be blank).");
                              return View(blogPost);
                        }
                        if (blogPost.Slug != slug)
                        {
                              //determine whether or not the slug has already been used
                              if (db.Posts.Any(p => p.Slug == slug))
                              {
                                    ModelState.AddModelError("", "Invalid entry (possible duplicate).");
                                    return View(blogPost);
                              }
                        }

                        if (ImageUploadValidator.IsWebFriendlyImage(image))
                        {
                              var fileName = Path.GetFileName(image.FileName);
                              image.SaveAs(Path.Combine(Server.MapPath("~/Uploads/" + fileName)));
                              blogPost.MediaPath = "/Uploads/" + fileName;
                        }

                        blogPost.Slug = slug;
                        db.Entry(blogPost).State = EntityState.Modified;
                        db.SaveChanges();
                        return RedirectToAction("Index");
                  }
                  return View(blogPost);
            }

            // GET: BlogPosts/Delete/5
            [Authorize(Roles = "Admin, Moderator")]
            public ActionResult Delete(int? id)
            {
                  if (id == null)
                  {
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                  }
                  BlogPost blogPost = db.Posts.Find(id);
                  if (blogPost == null)
                  {
                        return HttpNotFound();
                  }
                  return View(blogPost);
            }

            // POST: BlogPosts/Delete/5
            [HttpPost, ActionName("Delete")]
            [ValidateAntiForgeryToken]
            public ActionResult DeleteConfirmed(int id)
            {
                  BlogPost blogPost = db.Posts.Find(id);
                  db.Posts.Remove(blogPost);
                  db.SaveChanges();
                  return RedirectToAction("Index");
            }

            protected override void Dispose(bool disposing)
            {
                  if (disposing)
                  {
                        db.Dispose();
                  }
                  base.Dispose(disposing);
            }
      }
}
