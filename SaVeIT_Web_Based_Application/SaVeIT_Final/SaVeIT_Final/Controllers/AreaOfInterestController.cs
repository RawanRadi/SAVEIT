using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Linq.Dynamic;
using SaVeIT_Final.Models;
using PagedList;
using System.Threading.Tasks;
using System.Net.Mail;
using System.Data.Entity;
using System.Web.UI;
using System.IO;

namespace SaVeIT_Final.Controllers
{
    public class AreaOfInterestController : Controller
    {
        public Entities db = new Entities();
        // GET: AreaOfInterest
        [HttpGet]
        public ActionResult Index()
        {

            /* No logic required here, let's just render the view */
            return View();
        }

        public string Index(FormCollection collection)
        {
            /* Setup default variables that we are going to populate later */
            var pag_content = "";
            var pag_navigation = "";

            /* Define all posted data coming from the view. */
            int page = Convert.ToInt32(collection["data[page]"]); /* Page we are currently at */


            int max = Convert.ToInt32(collection["data[max]"]); /* Number of items to display per page */
            string search = collection["data[search]"]; /* Keyword provided on our search box */

            int cur_page = page;
            page -= 1;
            int per_page = max > 1 ? max : 18;
            int start = page * (max > 1 ? max : 18);


            /* Let's build the query using available data that we received form the front-end via ajax */
            Entities db = new Entities();

            var all_items_query = db.AreaOFInterests.ToList()
                   .Skip(start)
                   .Take(per_page).ToList(); /* Get only the items to display. */

            /* Get total items in our database */
            var count_query = db.AreaOFInterests
                .Where(x => x != null); /* Get total products count. */

            /* If there is a search keyword, we search through the database for possible matches*/
            if (search != "")
            {
                var temp = db.AreaOFInterests.Where(x => x.AOIName.ToUpper().Contains(search.ToUpper()));

                /* The "Contains" method matches records using the LIKE %keyword% format */
                all_items_query = temp.ToList();


                count_query = temp;
            }

            /* We now fetch the data from our database */
            var all_items = all_items_query;
            int count = count_query.Count();

            if (count > 0)
            {
                /* Loop through each item to create views */
                foreach (var item in all_items)
                {

                    pag_content += $"{$"<div class='contaner'><div class='box'><img src ='data:image;base64,"}{@Convert.ToBase64String(@item.AOIIcon)} 'width ='60' height ='60'/><hr/><p id ='SearchData'> {@item.AOIName}</p><a href=\"/AreaOfInterest/EachArea/{@item.AOIId}\"> See Projects </a></div></div>";

                }
            }
            else
            {
                /* Show a message if no items were found */
                pag_content += "<p class='p-d bg-danger'>No items found</p>";
            }

            pag_content = pag_content + "<br class = 'clear' />";

            /* Bellow is the navigation logic and view */
            decimal nop_ceil = decimal.Divide(d1: count, d2: max > 1 ? max : 18);

            var start_loop = 1;
            var end_loop = Convert.ToInt32(Math.Ceiling(nop_ceil));

            if (cur_page >= 7)
            {
                start_loop = cur_page - 3;
                if (Convert.ToInt32(Math.Ceiling(nop_ceil)) > cur_page + 3)
                {
                    end_loop = cur_page + 3;
                }
                else if (cur_page <= Convert.ToInt32(Math.Ceiling(nop_ceil)) && cur_page > Convert.ToInt32(Math.Ceiling(nop_ceil)) - 6)
                {
                    start_loop = Convert.ToInt32(Math.Ceiling(nop_ceil)) - 6;
                    end_loop = Convert.ToInt32(Math.Ceiling(nop_ceil));
                }
            }
            else
            {
                if (Convert.ToInt32(Math.Ceiling(nop_ceil)) > 7)
                {
                    end_loop = 7;
                }
            }

            pag_navigation += "<ul>";

            if (true && cur_page > 1)
            {
                pag_navigation += "<li p='1' class='active'>First</li>";
            }
            else if (true)
            {
                pag_navigation += "<li p='1' class='inactive'>First</li>";
            }

            if (true && cur_page > 1)
            {
                var pre = cur_page - 1;
                pag_navigation += "<li p='" + pre + "' class='active'>Previous</li>";
            }
            else if (true)
            {
                pag_navigation += "<li class='inactive'>Previous</li>";
            }

            for (int i = 1; i <= Convert.ToInt32(Math.Ceiling(nop_ceil)); i++)
            {

                if (cur_page == i)
                {
                    pag_navigation += "<li p='" + i + "' class = 'selected' >" + i + "</li>";
                }
                else
                    pag_navigation += "<li p='" + i + "' class='active'>" + i + "</li>";
            }

            if (true && cur_page < Convert.ToInt32(Math.Ceiling(d: nop_ceil)))
            {
                pag_navigation += "<li p='" + (cur_page + 1) + "' class='active'>Next</li>";
            }
            else if (true)
            {
                pag_navigation += "<li class='inactive'>Next</li>";
            }

            if (true && cur_page < Convert.ToInt32(Math.Ceiling(d: nop_ceil)))
            {
                pag_navigation += "<li p='" + Convert.ToInt32(Math.Ceiling(nop_ceil)) + "' class='active'>Last</li>";
            }
            else if (true)
            {
                pag_navigation += "<li p='" + Convert.ToInt32(Math.Ceiling(nop_ceil)) + "' class='inactive'>Last</li>";
            }

            pag_navigation = pag_navigation + "</ul>";

            /* Lets put our variables in a dictionary */

            /* Then we return the Dictionary in json format to our front-end */
            return new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(new Dictionary<string, string> {
        { "content", pag_content },
        { "navigation", pag_navigation }
    });
        }

        private static string HtmlPageContent(AreaOFInterest item, string pag_content)
        {
            pag_content += $"{$"<div class='box'><img src ='data:image;base64,"}{@Convert.ToBase64String(@item.AOIIcon)} 'width ='60' height ='60'/><hr/><p id ='SearchData'> {@item.AOIName}</p><a href=\"/AreaOfInterest/EachArea/{@item.AOIId}\"> See Projects </a></div>";
            return pag_content;
        }

        public ActionResult Show(int? id)
        {

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var sp = (from m in db.SeniorProjects
                      join s in db.ProjectsAOIs on m.SPtId equals s.SPtId
                      where s.AOIId == id
                      select m
            );
            if (sp == null)
            {

                return HttpNotFound();
            }
            return View(sp.ToList());

        }

        
        public ActionResult EachArea(int? id, string Sorting_Order, string Search_Data, string Filter_Value, string Searchby, int? Page_No)
        {
            ViewBag.CurrentSortOrder = Sorting_Order;
            ViewBag.SortingName = String.IsNullOrEmpty(Sorting_Order) ? "Name" : "";
            ViewBag.SortingDate = Sorting_Order == "Year" ? "descending year" : "Year";

            if (Search_Data != null)
            {
                Page_No = 1;
            }
            else
            {
                Search_Data = Filter_Value;
            }

            ViewBag.FilterValue = Search_Data;

            var students = (from m in db.SeniorProjects
                            join s in db.ProjectsAOIs on m.SPtId equals s.SPtId
                            where s.AOIId == id
                            select m
            );

            
            bool sby = false;
            if (Searchby == "1")//title
            {
                students = students.Where(stu => stu.SPName.ToUpper().Contains(Search_Data.ToUpper()));
                sby = true;
            }
            if (Searchby == "2")//year
            {
              students = students.Where(stu => stu.Year.ToString().Contains(Search_Data.ToUpper()));
                sby = true;

            }
            if (Searchby == "3")//programming l
            {
                students = students.Where(stu => stu.progLang.ToUpper().Contains(Search_Data.ToUpper()));
                sby = true;

            }
            if (!String.IsNullOrEmpty(Search_Data)&& sby==false)
            {
                students = students.Where(stu => stu.SPName.ToUpper().Contains(Search_Data.ToUpper()));
            }

            switch (Sorting_Order)
            {
                case "Name":
                    students = students.OrderByDescending(stu => stu.SPName);
                    break;
                case "Year":
                    students = students.OrderBy(stu => stu.Year);
                    break;
                case "descending year":
                    students = students.OrderByDescending(stu => stu.Year);
                    break;
                default:
                    students = students.OrderBy(stu => stu.SPName);
                    break;
            }


            int Size_Of_Page = 6;
            int No_Of_Page = (Page_No ?? 1);
            return View(students.ToPagedList(No_Of_Page, Size_Of_Page));
        }

        public ActionResult EachAreaAdmin(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var sp = (from m in db.SeniorProjects
                      join s in db.ProjectsAOIs on m.SPtId equals s.SPtId
                      where s.AOIId == id
                      select m
            );
            if (sp == null)
            {
                return HttpNotFound();
            }
            return View(sp.ToList());
        }

        public ActionResult EachAreaAdmin2(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var sp = (from m in db.SeniorProjects
                      join s in db.ProjectsAOIs on m.SPtId equals s.SPtId
                      where s.AOIId == id
                      select m
            );
            if (sp == null)
            {
                return HttpNotFound();
            }
            return View(sp);
        }


        public ActionResult Project(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SeniorProject seniorProject = db.SeniorProjects.Find(id);
            if (seniorProject == null)
            {
                return HttpNotFound();
            }
            SeniorProject s = new SeniorProject()
            {
                SPVideos = seniorProject.SPVideos,
                SPName = seniorProject.SPName,
                SPAbstract = seniorProject.SPAbstract,
                progLang = seniorProject.progLang,
                User = seniorProject.User,
                Users = (from ss in db.Users
                          where ss.projectGroup == id
                          select ss).ToList(), // To select group members in the project
            };

            return View(s);

        }


        public ActionResult Add()
        {
            AreaOFInterest AOI = new AreaOFInterest();
            AreaOfInterest2 model2 = new AreaOfInterest2();

            return View(model2);
        }
        [HttpPost]
        public ActionResult Add(AreaOfInterest2 model, HttpPostedFileBase image1)
        {
            try
            {
                AreaOFInterest model2 = new AreaOFInterest();
                var db = new Entities();
                if (ModelState.IsValid)
                {
                    if (image1 != null)
                    {
                        string ext = Path.GetExtension(image1.FileName).ToUpper();
                        if (ext == ".PNG" || ext == ".JPG" || ext == ".JPEG")
                        {
                            model2.AOIIcon = new byte[image1.ContentLength];
                            image1.InputStream.Read(model2.AOIIcon, 0, image1.ContentLength);
                            model2.AOIName = model.AOIName;
                            db.AreaOFInterests.Add(model2);
                            db.SaveChanges();
                            return Content("<script language='javascript' type='text/javascript'>alert('New area of interest added successfully');window.location = '/AreaOfInterest/AOIAdmin/';</script>");
                            //return RedirectToAction("AOIAdmin");
                        }
                        else
                        {
                            ViewBag.ErrorMessage = "The icon should be .png or .jpg or .jpeg";
                        }
                    }
                    else
                    {
                        ViewBag.ErrorMessages = "Upload icon of area of interest";
                    }
                }
                return View(model);
            }
            catch (Exception ex)
            {
                return View("Error", new HandleErrorInfo(ex, "AreaOfInterest", "Add"));
            }
        }

        // GET: AreaOFInterestsmmmmm/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AreaOFInterest areaOFInterest = db.AreaOFInterests.Find(id);
            if (areaOFInterest == null)
            {
                return HttpNotFound();
            }
            return View(areaOFInterest);
        }

        // POST: AreaOFInterestsmmmmm/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "AOIId,AOIName,AOIIcon")] AreaOFInterest areaOFInterest, HttpPostedFileBase image1)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var db = new Entities();
                    if (image1 != null)
                    {
                        string ext = Path.GetExtension(image1.FileName).ToUpper();
                        if (ext == ".PNG" || ext == ".JPG" || ext == ".JPEG")
                        {
                            areaOFInterest.AOIIcon = new byte[image1.ContentLength];
                            image1.InputStream.Read(areaOFInterest.AOIIcon, 0, image1.ContentLength);
                        }
                        else
                        {
                            ViewBag.ErrorMessage = "The icon should be .png or .jpg or .jpeg";
                        }

                    }
                    db.Entry(areaOFInterest).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("AOIAdmin");
                }
                return View(areaOFInterest);
            }
            catch (Exception ex)
            {
                return View("Error", new HandleErrorInfo(ex, "AreaOfInterest", "Edit"));
            }
        }


        public ActionResult Delete(int? id)
        {
            try
            {
                if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AreaOFInterest AOI = db.AreaOFInterests.Find(id);
            if (AOI == null)
            {
                return HttpNotFound();
            }
            else
            {
                db.AreaOFInterests.Remove(AOI);
                db.SaveChanges();
            }
            return RedirectToAction("AOIAdmin");
            }
            catch (Exception ex)
            {
                return View("Error", new HandleErrorInfo(ex, "AreaOfInterest", "Delete"));
            }
        }

        // POST: AreaOFInterestsmmmmm/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
                AreaOFInterest areaOFInterest = db.AreaOFInterests.Find(id);

                db.AreaOFInterests.Remove(areaOFInterest);
                db.SaveChanges();
                return RedirectToAction("Index");
            
        }
        public ActionResult loaddata()
        {
            using (Entities dc = new Entities())
            {
                // dc.Configuration.LazyLoadingEnabled = false; // if your table is relational, contain foreign key
                var temp = dc.AreaOFInterests.OrderBy(a => a.AOIName).ToList();
                var data = temp.Select(S => new { AOIName = S.AOIName, AOIId = S.AOIId });
                return Json(new { data = data }, JsonRequestBehavior.AllowGet);
            }
        }


        public ActionResult loadProject()
        {
            using (Entities dc = new Entities())
            {
                // dc.Configuration.LazyLoadingEnabled = false; // if your table is relational, contain foreign key
                var temp = dc.SeniorProjects.OrderBy(a => a.SPtId).ToList();
                var data = temp.Select(S => new { SPName = S.SPName, SPId = S.SPtId });
                return Json(new { data = data }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult loadEachArea(int? id)
        {
            using (Entities dc = new Entities())
            {
                var sp = (from m in dc.SeniorProjects
                          join s in dc.ProjectsAOIs on m.SPtId equals s.SPtId
                          where s.AOIId == id
                          select m);
                // dc.Configuration.LazyLoadingEnabled = false; // if your table is relational, contain foreign key
                var temp = sp.OrderBy(a => a.SPtId).ToList();
                var data = sp.Select(S => new { SPName = S.SPName, SPId = S.SPtId });
                return Json(new { data = data }, JsonRequestBehavior.AllowGet);
            }
        }


        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Contact(int id, String FromName, String FromEmail, String Message, String subject)
        {

            //To get email addresses of students in the project
            var studentEmail = (from ss in db.Users
                                where ss.projectGroup == id
                                select ss.userEmail).ToArray();
            var isSuccess = false;
            var message = "";
            using (var smtp = new SmtpClient())
            {

                for (int h = 0; h < studentEmail.Length; h++)
                {
                    try
                    {
                        var body = "<p>Email From: {0} ({1})</p><p>Message:</p><p>{2}</p>";
                        var smtpClient = new SmtpClient();
                        var msg = new MailMessage();

                        msg.To.Add(new MailAddress(studentEmail[h])); //replace with valid value

                        msg.Subject = subject;
                        msg.Body = string.Format(body, FromName, FromEmail, Message);
                        msg.IsBodyHtml = true;

                        smtp.Send(msg);
                        isSuccess = true;
                        message = "sent";
                    }
                    catch (Exception)
                    {
                        message = "Sending failed";
                        //MessageBox("Your Email address is not valid");
                        //ScriptManager.RegisterStartupScript(this, this.GetType(), "popup", "alert('Your Email address is not valid');", true);
                    }
                }
            }
            var jsonData = new { isSuccess, message };

            return Json(jsonData);
        }

        public ActionResult AOIAdmin()
        {
            return View(db.AreaOFInterests.ToList());
        }

        public ActionResult AOIAdmin2()
        {
            return View(db.AreaOFInterests.ToList());
        }

        public ActionResult ProjectAdmin()
        {
            return View(db.SeniorProjects.ToList());
        }


    }


}
