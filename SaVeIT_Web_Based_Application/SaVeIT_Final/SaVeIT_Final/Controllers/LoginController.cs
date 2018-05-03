using System.Linq;
using System.Web.Mvc;
using SaVeIT_Final.Models;

namespace SaVeIT_Final.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(User2 model)
        {
            User model2 = new User();
            using (Entities db = new Entities())
            {
                var UserDetails = db.Users.Where(x => x.userId == model.userId && x.password == model.password).FirstOrDefault();
                if (db.Users.Where(x => x.userId == model.userId && x.password != model.password).FirstOrDefault() != null)
                {//if user id is correct but not match with the password
                    ViewBag.ErrorMessage = "The password is wrong";
                }
                else if (db.Users.Where(x => x.userId != model.userId).FirstOrDefault() != null)
                {//if the user id does not exit in DB
                    ViewBag.ErrorMessage = "Check the User ID because It is not Found!";
                }
                if (UserDetails != null)
                { //if user id and password is correct will save user information into th session
                    Session["UserID"] = UserDetails.userId.ToString();
                    Session["UserName"] = UserDetails.userName;

                    return RedirectToAction("Index", "Home");
                }
            }
            return View(model);
        }

    }
}