using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SaVeIT_Final.Models;

namespace SaVeIT_Final.Controllers
{
    public class SupervisorsController : Controller
    {
        private Entities db = new Entities();

        [HttpGet]
        public ActionResult Index()
        {
            /* No logic required here, let's just render the view */
            return View();
        }

        [HttpPost]
        public string Index(FormCollection collection)
        {
            /* Setup default variables that we are going to populate later */
            var pag_content = "";
            var pag_navigation = "";

            /* Define all posted data coming from the view. */
            int page = Convert.ToInt32(collection["data[page]"]); /* Page we are currently at */

            string searchby = collection["data[searchby]"];

            int max = Convert.ToInt32(collection["data[max]"]); /* Number of items to display per page */
            string search = collection["data[search]"]; /* Keyword provided on our search box */

            int cur_page = page;
            page -= 1;
            int per_page = max > 1 ? max : 18;
            bool previous_btn = true;
            bool next_btn = true;
            bool first_btn = true;
            bool last_btn = true;
            int start = page * per_page;



            /* Let's build the query using available data that we received form the front-end via ajax */
            Entities db = new Entities();
            var role = (from r in db.Users
                        where r.userRole == 2 || r.userRole == 1
                        select r
                   );

            var all_items_query = role.ToList()
                .Skip(start)
                .Take(per_page).ToList(); /* Get only the products to display. */

            /* Get total items in our database */
            var count_query = role
                .Where(x => x != null); /* Get total products count. */

            /* If there is a search keyword, we search through the database for possible matches*/
            if (search != "")
            {
                if (searchby.ToString() == "name")
                {
                    var temp = role.Where(x => x.userName.ToUpper().Contains(search.ToUpper()));
                    /* The "Contains" method matches records using the LIKE %keyword% format */
                    all_items_query = temp.ToList();
                    count_query = temp;

                }
                if (searchby.ToString() == "Area")
                {
                    var temp = role.Where(x => x.SupAOIs.Any(y => y.AreaOFInterest.AOIName.ToUpper().Contains(search.ToUpper())));
                    all_items_query = temp.ToList();
                    count_query = temp;
                }



            }

            /* We now fetch the data from our database */
            var all_items = all_items_query;
            int count = count_query.Count();

            if (count > 0)
            {
                /* Loop through each item to create views */
                foreach (var item in all_items)
                {

                    pag_content += $"<a href =\"/Supervisors/Details/{@item.userId}\"> <div class='col-sm-3'><div class='card'><canvas class='header-bg' width='250' height='70' id='header-blur'></canvas><div class='avatar'>  <img src = '/images/female-worker.png' alt=''/></div><div class='content'> <br/><strong>{item.userName}</strong>  </div> </div> </div></a>";
                }
            }
            else
            {
                /* Show a message if no items were found */
                pag_content += "<p class='p-d bg-danger'>No items found</p>";
            }

            pag_content = pag_content + "<br class = 'clear' />";

            /* Bellow is the navigation logic and view */
            decimal nop_ceil = Decimal.Divide(count, per_page);
            int no_of_paginations = Convert.ToInt32(Math.Ceiling(nop_ceil));

            var start_loop = 1;
            var end_loop = no_of_paginations;

            if (cur_page >= 7)
            {
                start_loop = cur_page - 3;
                if (no_of_paginations > cur_page + 3)
                {
                    end_loop = cur_page + 3;
                }
                else if (cur_page <= no_of_paginations && cur_page > no_of_paginations - 6)
                {
                    start_loop = no_of_paginations - 6;
                    end_loop = no_of_paginations;
                }
            }
            else
            {
                if (no_of_paginations > 7)
                {
                    end_loop = 7;
                }
            }

            pag_navigation += "<ul>";

            if (first_btn && cur_page > 1)
            {
                pag_navigation += "<li p='1' class='active'>First</li>";
            }
            else if (first_btn)
            {
                pag_navigation += "<li p='1' class='inactive'>First</li>";
            }

            if (previous_btn && cur_page > 1)
            {
                var pre = cur_page - 1;
                pag_navigation += "<li p='" + pre + "' class='active'>Previous</li>";
            }
            else if (previous_btn)
            {
                pag_navigation += "<li class='inactive'>Previous</li>";
            }

            for (int i = start_loop; i <= end_loop; i++)
            {

                if (cur_page == i)
                    pag_navigation += "<li p='" + i + "' class = 'selected' >" + i + "</li>";
                else
                    pag_navigation += "<li p='" + i + "' class='active'>" + i + "</li>";
            }

            if (next_btn && cur_page < no_of_paginations)
            {
                var nex = cur_page + 1;
                pag_navigation += "<li p='" + nex + "' class='active'>Next</li>";
            }
            else if (next_btn)
            {
                pag_navigation += "<li class='inactive'>Next</li>";
            }

            if (last_btn && cur_page < no_of_paginations)
            {
                pag_navigation += "<li p='" + no_of_paginations + "' class='active'>Last</li>";
            }
            else if (last_btn)
            {
                pag_navigation += "<li p='" + no_of_paginations + "' class='inactive'>Last</li>";
            }

            pag_navigation = pag_navigation + "</ul>";

            /* Lets put our variables in a dictionary */
            var response = new Dictionary<string, string> {
        { "content", pag_content },
        { "navigation", pag_navigation }
    };

            /* Then we return the Dictionary in json format to our front-end */
            string json = new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(response);
            return json;
        }

public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User supervisorr = db.Users.Find(id);

            if (supervisorr == null)
            {

                return HttpNotFound();
            }

            User su = new User()
            {
                userName = supervisorr.userName,
                userEmail = supervisorr.userEmail,
                SeniorProjects = db.SeniorProjects.Where(s => s.supervisorUserId == id).ToList(),
            };

            var area = (from m in db.AreaOFInterests
                        join s in db.SupAOIs on m.AOIId equals s.AOIId
                        where s.supervisorUserId == id
                        select m).ToList();

            supervisor1 supervisor = new supervisor1()
            {
                userName = supervisorr.userName,
                userEmail = supervisorr.userEmail,
                SeniorProjects = db.SeniorProjects.Where(s => s.supervisorUserId == id).ToList(),
                AOIs = area
            };


            return View(supervisor);

        }
        /// <summary>
        /// index page for admin
        /// </summary>
        /// <returns></returns>
        public ActionResult Admin()
        {
            var supervisors = db.Users.Where(s => s.userRole == 2 || s.userRole == 1);

            return View(supervisors);
        }

        // GET: Users/Create
        public ActionResult Create()
        {
            ViewBag.userRole = new SelectList(db.UserRoles.Where(r => r.userId == 1 || r.userId == 2), "userId", "userName");
            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "userId,userName,userEmail,userRole,password")] User user)
        {
            try
            {
                User u = db.Users.Find(user.userId);
                if (u != null)
                {
                    return Content("< script language='javascript' type = 'text/javascript' > alert('The supervisor Already exists'); </script>");

                }
                if (user == null)
                {
                    return Content("< script language='javascript' type = 'text/javascript' > alert('The supervisor does not found') </script>");
                }
                if (ModelState.IsValid)
                {
                    db.Users.Add(user);
                    db.SaveChanges();
                    return Content("<script language='javascript' type='text/javascript'>alert('New supervisor added successfully');window.location = '/Supervisors/Admin';</script>");

                }

                ViewBag.userRole = new SelectList(db.UserRoles.Where(r => r.userId == 1 || r.userId == 2), "userId", "userName", user.userRole);
                return View(user);
            }
            catch (Exception e)
            {
                return Content("< script language='javascript' type = 'text/javascript' > alert('Error occurred while creating a record'  " + e + ") </script>");

            }
        }

        public ActionResult Delete(string id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                User user = db.Users.Find(id);
                if (user == null)
                {
                    return HttpNotFound();
                }
                else
                {
                    db.Users.Remove(user);
                    db.SaveChanges();
                    // return Content("<script language='javascript' type='text/javascript'>alert('Supervisor delete successfully');window.location = '/Supervisors/Admin';</script>");
                    return RedirectToAction("Admin");

                }
               // return RedirectToAction("Admin");
            }
            catch (Exception e)
            {
                return Content("< script language='javascript' type = 'text/javascript' > alert('Error occurred while deleting record'" + e + ") </script>");
            }
        }
        // GET: Supervisors/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            ViewBag.userRole = new SelectList(db.UserRoles.Where(r => r.userId == 1 || r.userId == 2), "userId", "userName", user.userRole);
            return View(user);
        }

        // POST: Supervisors/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "userId,userName,userEmail,userRole,password")] User user)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.Entry(user).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Admin");
                }
                ViewBag.userRole = new SelectList(db.UserRoles.Where(r => r.userId == 1 || r.userId == 2), "userId", "userName", user.userRole);
                return View(user);
            }
            catch (Exception e)
            {
                return Content("< script language='javascript' type = 'text/javascript' > alert('Error occurred while updating record'" + e + ") </script>");

            }
        }

        public ActionResult loaddata()
        {
            try
            {
                using (Entities dc = new Entities())
                {
                    // dc.Configuration.LazyLoadingEnabled = false; // if your table is relational, contain foreign key
                    var temp = dc.Supervisors.OrderBy(a => a.supervisorName).ToList();
                    var data = temp.Select(S => new { supervisorName = S.supervisorName, supervisorId = S.supervisorId });
                    return Json(new { data = data }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {
                return Content("< script language='javascript' type = 'text/javascript' > alert('Error occurred while loading the data'" + e + ") </script>");

            }
        }

        public ActionResult loadSupervisorData()
        {
            try
            {
                using (Entities dc = new Entities())
                {
                    var temp = dc.Users.Where(s => s.userRole == 2 || s.userRole == 1).OrderBy(a => a.userName).ToList();
                    var data = temp.Select(S => new { userName = S.userName, userId = S.userId });
                    return Json(new { data = data }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {
                return Content("< script language='javascript' type = 'text/javascript' > alert('Error occurred while loading the data'" + e + ") </script>");

            }
        }

    }
}