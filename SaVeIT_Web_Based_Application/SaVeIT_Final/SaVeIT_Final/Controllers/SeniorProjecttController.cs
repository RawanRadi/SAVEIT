//SeniorProjecttController

using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SaVeIT_Final.Helpers;
using SaVeIT_Final.Models;
using SaVeIT_Final.ViewModel;

namespace SaVeIT_Final.Controllers
{
    public class SeniorProjecttController : Controller
    {

        Entities db = new Entities();
        public ActionResult Projects()
        {
            var sp = (from m in db.SeniorProjects
                      select m);

            return View(sp);
        }


        [HttpGet]
        public ActionResult DownLoadFile(int id)
        {
            var FileById = (from FC in db.SeniorProjects
                            where FC.SPtId.Equals(id)
                            select new { FC.SPReportName, FC.SPReport }).ToList().FirstOrDefault();
            try
            {
                return File(FileById.SPReport, "application/pdf", FileById.SPReportName);
            }
            catch (ArgumentNullException)
            {

                return Content("<script language='javascript' type='text/javascript'>alert('Senior project report not found!');window.location = '/AreaOfInterest/Project/" + id + "';</script>");

            }
        }

        // GET: Upload   
        public ActionResult ReportUpload()
        {
            return View();
        }
        /***********************************************************************************************************************************************/
        //FileUpload function to upload reports
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ReportUpload(HttpPostedFileBase file)
        {
            int projectID = 0;
            try
            {
                if (file != null && file.ContentLength > 0)
                {
                    var contentLength = file.ContentLength;
                    var contentType = file.ContentType;
                    DateTime now = DateTime.Now;
                    ViewBag.PTitlE = Path.GetFileNameWithoutExtension(file.FileName);
                    string extenstion = "";
                    if (Path.GetExtension(file.FileName).ToUpper() == ".PDF")
                    {
                        // Get file data
                        byte[] data = new byte[] { };
                        using (var binaryReader = new BinaryReader(file.InputStream))
                        {
                            data = binaryReader.ReadBytes(file.ContentLength);
                        }
                        var SeniorProject = new SeniorProject()
                        {
                            // SPName = report.FileName,
                            SPName = Path.GetFileNameWithoutExtension(file.FileName),
                            SPReport = data,

                            /// supervisorId = "10",
                        };
                        using (Entities entity = new Entities())
                        {
                            entity.SeniorProjects.Add(SeniorProject);
                            entity.SaveChanges();
                        }
                        projectID = SeniorProject.SPtId;
                        return new JsonResult
                        {
                            Data = "file uploaded successfully"
                        };
                    }
                    else
                    {
                        extenstion = (Path.GetExtension(file.FileName) + "not allowed only .pdf files");
                        return new JsonResult
                        {
                            Data = extenstion
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                return new JsonResult
                {
                    Data = "an error occuerd"
                };
            }
            return new JsonResult
            {
                Data = "file is empety"
            };
        }    // }
             /**********************************************************/
             /*****************submit function for supervisor form************************/
        [HttpPost]
        public ActionResult AddSupervisortoProject(ProjectVM model)
        {     // assigen supervisor id to project  
            if (model.supervisor.userId != null)
            {
                //first check if this supervisor exists in the db or not
                //   Supervisor supervisor = db.Supervisors.Find(model.supervisorId);

                User supervisor = db.Users.Find(model.supervisor.userId);
                if (supervisor == null)
                {
                    //supervisor = new Supervisor();
                    supervisor = new User();
                    supervisor.userId = model.userId;
                    supervisor.userName = model.userName;
                    supervisor.userRole = 2;
                    using (Entities entity = new Entities())
                    {
                        entity.Users.Add(supervisor);
                        entity.SaveChanges();
                    }
                }


                SeniorProject project = db.SeniorProjects.Find(model.SPtId);
                project.supervisorUserId = model.supervisor.userId;
                db.Entry(project).State = EntityState.Modified;
                db.SaveChanges();

                //List of aoi of this project
                List<int> areaId = new List<int>();
                areaId = (from FC in db.ProjectsAOIs
                          where FC.SPtId == model.SPtId
                          select FC.AOIId).ToList();
                //List of aoi of this supervisor
                List<int> SupareaId = new List<int>();
                SupareaId = (from FC in db.SupAOIs
                             where FC.supervisorUserId == model.supervisor.userId
                             select FC.AOIId).ToList();
                //to find aoi for this project but not assigened to this supervisor  
                IEnumerable<int> AOIDifference;
                AOIDifference = areaId.Except(SupareaId);
                // assigen area of interest to this supervisor 
                foreach (var item in AOIDifference)
                {
                    SupAOI saoi = new SupAOI();
                    saoi.AOIId = item;
                    saoi.supervisorUserId = model.supervisor.userId;
                    using (Entities entity = new Entities())
                    {
                        entity.SupAOIs.Add(saoi);
                        entity.SaveChanges();
                    }
                }
                /*return new JsonResult
                {
                    Data = "Saved successfully"
                };*/

            }
            return new JsonResult
            {
                Data = "Saved successfully"
            };
        }
        /*****************submit function for report form************************/
        [HttpPost]
        public ActionResult SaveReport(ProjectVM model)
        {
            try
            {
                AreaOFInterest aoi = new AreaOFInterest();
                ProjectsAOI paoi = new ProjectsAOI();
                List<int> Ids = new List<int>();

                if (model.AOIList != null)
                {
                    if (model.AOIList.Count > 0)
                    {

                        for (var i = 0; i < model.AOIList.Count; i++)
                        {

                            var item = model.AOIList.ElementAt(i).Trim().ToUpper();

                            var area = from s in db.AreaOFInterests where s.AOIName.ToUpper() == item select s;
                            //if area not found it will be added to the table
                            if (!area.Any())
                            {
                                aoi.AOIName = model.AOIList.ElementAt(i).Trim();

                                using (Entities entity = new Entities())
                                {
                                    entity.AreaOFInterests.Add(aoi);
                                    entity.SaveChanges();
                                }
                            }
                        }
                        for (var i = 0; i < model.AOIList.Count; i++)
                        {

                            var item = model.AOIList.ElementAt(i).Trim().ToUpper();

                            int areaId = (from FC in db.AreaOFInterests
                                          where FC.AOIName.ToUpper() == item
                                          select FC.AOIId).ToList().FirstOrDefault();

                            //assigen area to tables
                            if (areaId != 0)
                            {
                                paoi.AOIId = areaId;
                                paoi.SPtId = model.SPtId;
                                using (Entities entity = new Entities())
                                {
                                    entity.ProjectsAOIs.Add(paoi);
                                    entity.SaveChanges();
                                }
                            }
                        }


                    }
                }
                Award award = new Award();

                if (model.AwardsList != null)
                {
                    if (model.AwardsList.Count > 0)
                    {
                        for (int i = 0; i < model.AwardsList.Count; i++)
                        {
                            award.awardName = model.AwardsList.ElementAt(i).Trim();

                            using (Entities entity = new Entities())
                            {
                                entity.Awards.Add(award);
                                entity.SaveChanges();
                            }
                        }
                        /*********************************Add awards to projectAward table (to mapping between project and award)*******************************************************************/
                        ProjectAward projectAward = new ProjectAward();
                        for (int i = 0; i < model.AwardsList.Count; i++)
                        {
                            var item = model.AwardsList.ElementAt(i).Trim();
                            //get IDs of each award
                            int? awardId = (from FC in db.Awards
                                            where FC.awardName == item
                                            select FC.awardId).ToList().FirstOrDefault();
                            projectAward.ProjectId = model.SPtId;
                            projectAward.AwardId = (int)awardId;
                            //assigen awards to mapping tables
                            if (awardId != null)
                            {
                                using (Entities entity = new Entities())
                                {
                                    entity.ProjectAwards.Add(projectAward);
                                    entity.SaveChanges();
                                }
                            }


                        }
                    }
                }
                string PL = "";
                //set programming languages in one string 
                if (model.PLList != null)
                {
                    if (model.PLList.Count > 0)
                    {
                        for (int i = 0; i < model.PLList.Count - 1; i++)
                        {
                            PL += model.PLList.ElementAt(i).Trim().ToLower() + ",";

                        }
                        PL += model.PLList.ElementAt(model.PLList.Count - 1).Trim().ToLower();
                    }
                }

                ProjectEdit(model.SPtId, model.SPName, model.SPVideos, model.SPAbstract, model.SPGrade, PL);
                return new JsonResult
                {
                    Data = "Saved successfully"
                };

            }
            catch (Exception ex)
            {
                return new JsonResult
                {
                    Data = "an error occurred " + ex.Message
                };
            }

        }

        /*****************submit function for student form************************/
        [HttpPost]
        public ActionResult AddStudents(ProjectVM model)
        {
            User stu = new User();
            string message = "";
            try
            {
                for (var i = 0; i < model.Users.Count; i++)
                {

                    var item = model.Users.ElementAt(i);
                    stu.userId = item.userId;
                    stu.userName = item.userName;
                    stu.userEmail = item.userEmail;
                    stu.projectGroup = model.SPtId;
                    stu.userRole = 3;

                    {
                        using (Entities entity = new Entities())
                        {
                            entity.Users.Add(stu);
                            entity.SaveChanges();


                        }
                    }


                }
                return new JsonResult
                {
                    Data = "Students has been added successfully"
                };
            }
            catch (DbUpdateException)
            {
                message = "An error occurred while insert the students";
                return new JsonResult
                {
                    Data = message
                };
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return new JsonResult
                {
                    Data = message
                };
            }
        }


        /***********************************************************************************************************************************************/
        //Remote validation that validate if a section exists in the entered year to ensure no duplictae in the reports  
        [AllowAnonymous]
        public ActionResult IsSectionAvailable(ProjectValidation s)
        {
            //  if (!Year.HasValue)
            //   {
            //        return Json(true);
            //   }   
            if (s.Year == null)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
            return Json(!IsSectionExists(s.Section, s.Year), JsonRequestBehavior.AllowGet);
        }
        /***********************************************************************************************************************************************/
        private bool IsSectionExists(string Section, short? Year)
        {

            Section = Section.ToUpper();
            var project = from s in db.SeniorProjects where s.Section.ToUpper() == Section && s.Year == Year select s;
            return project.Any();
        }
        /***********************************************************************************************************************************************/
        //i have changed
        [HttpGet]
        public ActionResult SaveReportData(int? id)
        {

            // SeniorProject seniorProject = db.SeniorProject.OrderByDescending(o => o.SPtId).FirstOrDefault();
            SeniorProject seniorProject = new SeniorProject();
            seniorProject = db.SeniorProjects.Find(id);

            if (seniorProject == null)
            {
                return HttpNotFound();
            }
            ProjectVM project = new ProjectVM();
            project = GetStuFromPDF(seniorProject.SPtId);
            project.SPName = seniorProject.SPName;
            project.SPtId = seniorProject.SPtId;
            /*List<Student> students = new List<Student>();
            students=obj.StudentsAndID;
           /* Supervisors sup = new Supervisors();
            sup=obj.Sup;
             seniorProject.SPAbstract = obj.Project.SPAbstract;
            var TwoModel = new ProjectAndStudentModels { project = seniorProject, StudentsAndID = students, supervisors = sup };
           */

            return View(project);
        }
        /***********************************************************************************************************************************************/
        /*******************Extract students from pdf*****************************************************/
        public ProjectVM GetStuFromPDF(int? id)
        {

            ReportAnalysis report = new ReportAnalysis();
            ProjectVM obj = new ProjectVM();
            var FileById = (from FC in db.SeniorProjects
                            where FC.SPtId == id
                            select FC.SPReport).ToList().FirstOrDefault();

            byte[] fileBytes = FileById;
            report.Analysis(fileBytes, obj);

            return obj;
        }
        /***********************************************************************************************************************************************/
        /***********************************************************************************************************************************************/
        public ActionResult ErrorMessage(string msg)
        {
            ViewBag.MyExeption = msg;
            return View();
        }
        /***********************************************************************************************************************************************/

        public void ProjectEdit(int SPtId, string SPName, string link, string SPAbstract, string grade, string PL)
        {

            SeniorProject project = db.SeniorProjects.Find(SPtId);

            project.SPtId = SPtId;
            project.SPName = SPName;
            project.SPAbstract = SPAbstract;
            project.SPVideos = link;
            project.SPGrade = grade;
            project.progLang = PL;
            db.Entry(project).State = EntityState.Modified;
            db.SaveChanges();

        }
        /***********************************************************************************************************************************************/

        [HttpPost]
        public ActionResult InsertProject(ProjectValidation model)
        {
            SeniorProject Project = db.SeniorProjects.OrderByDescending(o => o.SPtId).FirstOrDefault();

            ProjectEdit(Project.SPtId, model.Section, (Int16)model.Year);
            int year = model.Year.Value;
            return RedirectToAction("SaveReportData", new { id = Project.SPtId });
        }
        public void ProjectEdit(int SPtId, string sec, short year)
        {

            SeniorProject project = db.SeniorProjects.Find(SPtId);

            project.SPtId = SPtId;
            project.Section = sec;
            project.Year = year;
            db.Entry(project).State = EntityState.Modified;
            db.SaveChanges();

        }


        [HttpPost]
        public JsonResult SearchAOI(string Prefix)
        {
            //Searching records for aoi using LINQ query  
            var AOIList = (from N in db.AreaOFInterests
                           where N.AOIName.StartsWith(Prefix)
                           select new { N.AOIName });
            return Json(AOIList, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult SearchSupervisors(string Prefix)
        {
            //Searching records for supervisors using LINQ query     
            var Sup = (from N in db.Users
                       where N.userName.Contains(Prefix) && (N.userRole == 1 || N.userRole == 2)
                       select new { Info = "Id of " + N.userName + " is " + N.userId });
            return Json(Sup, JsonRequestBehavior.AllowGet);
        }

        //@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@End SaveReport View functions@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@//

        /************Edit students update page************/
        public ActionResult Edit(int? id)
        {
            try
            {
                SeniorProject senproject = db.SeniorProjects.Find(id);
                ProjectVM projectvm = new ProjectVM();
                projectvm.Year = senproject.Year;
                projectvm.SPName = senproject.SPName;
                projectvm.SPtId = senproject.SPtId;
                projectvm.SPVideos = senproject.SPVideos;
                projectvm.SPAbstract = senproject.SPAbstract;
                projectvm.SPGrade = senproject.SPGrade;
                projectvm.Section = senproject.Section;
                projectvm.Year = senproject.Year;


                projectvm.PLList = new List<string>();
                if (senproject.progLang != null)
                {
                    List<string> progLang = senproject.progLang.Split(',').ToList();


                    projectvm.PLList = progLang;
                }


                List<string> AOI = new List<string>();
                AOI = (from FC in db.ProjectsAOIs
                       where FC.SPtId == id
                       select FC.AreaOFInterest.AOIName).ToList();

                projectvm.AOIList = new List<string>();


                projectvm.AOIList = AOI;


                List<string> Awards = new List<string>();
                Awards = (from FC in db.ProjectAwards
                          where FC.ProjectId == id
                          select FC.Award.awardName).ToList();
                projectvm.AwardsList = new List<string>();
                projectvm.AwardsList = Awards;

                /////////////////////////
                if (senproject.supervisorUserId != null)
                {
                    //projectvm.supervisor.userId = senproject.supervisorUserId;
                    //User supervisor = db.Users.Find(projectvm.supervisor.userId);
                    //projectvm.supervisor.userName = supervisor.userName;
                    projectvm.userId = senproject.supervisorUserId;
                    projectvm.userName = senproject.User.userName;
                }
                else
                {
                    //projectvm.supervisor.userId = "";
                    //projectvm.supervisor.userName = "";
                    projectvm.userId = "";
                    projectvm.userName = "";
                }
                ////////////////////////
                List<User> stu = (from FC in db.Users
                                  where FC.projectGroup == id
                                  select FC).ToList();
                projectvm.Users = new List<User>();
                if (stu != null)
                    projectvm.Users = stu;
                return View(projectvm);
            }
            catch (Exception ex)
            {
                return RedirectToAction("ErrorMessage", new { msg = "an error occurred" + ex.Message });
            }

        }
        //Remove Studnet from database
        [HttpPost]
        public ActionResult DeleteStudent(string id)
        {
            try
            {
                int.TryParse(id, out int temp);
                var student = new User { userId = temp + "" };
                if (id.Trim() == "")
                {
                    return new JsonResult
                    {
                        Data = "Student not exist in db"
                    };
                }
                else
                {
                    db.Entry(student).State = EntityState.Deleted;
                    db.SaveChanges();
                }
                return new JsonResult
                {
                    Data = "Student deleted"
                };
            }
            catch (Exception ex)
            {
                return new JsonResult
                {
                    Data = "an error occurred " + ex.Message
                };
            }
        }
        // update students
        [HttpPost]
        public ActionResult UpdateStudent(ProjectVM model)
        {
            try
            {
                List<User> students = new List<User>();
                students = model.Users;
                User stu = new User();

                foreach (var item in students)
                {
                    stu = db.Users.Find(item.userId);
                    if (stu == null)
                    {
                        stu = new User();
                        stu.userName = item.userName;
                        stu.userId = item.userId;
                        stu.userEmail = item.userEmail;
                        stu.projectGroup = model.SPtId;
                        stu.userRole = 3;
                        using (Entities entity = new Entities())
                        {
                            entity.Users.Add(stu);
                            entity.SaveChanges();
                        }
                    }
                    else
                    {
                        stu.userName = item.userName;
                        db.Entry(stu).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
                return new JsonResult
                {
                    Data = "Students edited successfully"
                };
            }
            catch (Exception ex)
            {
                return new JsonResult
                {
                    Data = "an error occurred " + ex.Message
                };
            }
        }
        /*****************End Edit students update page**********************************/
        [HttpPost]
        public ActionResult updateSupervisor(ProjectVM model)
        {
            try
            {
                SeniorProject project = db.SeniorProjects.Find(model.SPtId);
                project.supervisorUserId = model.supervisor.userId;
                User sup = db.Users.Find(model.userId);

                if (sup == null)
                {
                    sup = new User();
                    sup.userId = model.supervisor.userId;
                    sup.userId = model.supervisor.userName;
                    sup.userRole = model.supervisor.userRole = 2;
                    using (Entities entity = new Entities())
                    {
                        entity.Users.Add(sup);
                        entity.SaveChanges();
                    }
                }
                db.Entry(project).State = EntityState.Modified;
                db.SaveChanges();
                return new JsonResult
                {
                    Data = "Supervisor updated successfully"
                };
            }
            catch (Exception ex)
            {
                return new JsonResult
                {
                    Data = "an error occurred"
                };
            }
        }
        [HttpPost]
        public ActionResult UpdateSummaryReportTab(ProjectVM model, HttpPostedFileBase File)
        {
            try
            {

                ///////////////////////////AOI///////////////////////////////////////////////////////
                AreaOFInterest areaOFInterest = new AreaOFInterest();

                if (model.AOIList != null)
                {
                    if (model.AOIList.Count > 0)
                    {
                        // retrive all existing aoi of this project delete them then add all new aoi
                        List<ProjectsAOI> pAOI = (from s in db.ProjectsAOIs where s.SPtId == model.SPtId select s).ToList();
                        if (pAOI != null)
                        {
                            for (int item = 0; item < pAOI.Count; item++)
                            {
                                //Delete them from ProjectsAOI table
                                ProjectsAOI paoi = db.ProjectsAOIs.Find(pAOI[item].Id);
                                //   new ProjectsAOI { Id = pAOI[item].Id };
                                db.Entry(paoi).State = EntityState.Deleted;
                                db.SaveChanges();
                            }
                        }
                        // Now I will add every AOI again to mapping table
                        ProjectsAOI projectAOI = new ProjectsAOI();

                        for (int i = 0; i < model.AOIList.Count; i++)
                        {
                            string AOIName = model.AOIList.ElementAt(i).Trim().ToUpper();
                            var temp = (from s in db.AreaOFInterests where s.AOIName.ToUpper() == AOIName select s).FirstOrDefault();
                            //means it exists in AreaOfInterest table
                            if (AOIName != "")
                            {
                                if (temp != null)
                                {
                                    //get area id
                                    projectAOI.SPtId = model.SPtId;
                                    projectAOI.AOIId = temp.AOIId;
                                }
                                // else means no aoi has this name I will add it to area table then to mapping table
                                if (temp == null)
                                {
                                    AreaOFInterest newAOI = new AreaOFInterest();
                                    newAOI.AOIName = model.AOIList.ElementAt(i).Trim();
                                    using (Entities entity = new Entities())
                                    {
                                        entity.AreaOFInterests.Add(newAOI);
                                        entity.SaveChanges();
                                    }
                                    // get last added aoi then add it to aoi mapping table
                                    AreaOFInterest LastArea = db.AreaOFInterests.OrderByDescending(o => o.AOIId).FirstOrDefault();
                                    projectAOI.SPtId = model.SPtId;
                                    projectAOI.AOIId = LastArea.AOIId;
                                }
                                using (Entities entity = new Entities())
                                {
                                    entity.ProjectsAOIs.Add(projectAOI);
                                    entity.SaveChanges();
                                }
                            }
                        }

                    }
                }
                ///////////////////////////Awards///////////////////////////////////////////////////////


                if (model.AwardsList != null)
                {
                    if (model.AwardsList.Count > 0)
                    {
                        // retrive all existing awards of this project delete them then add all new awards
                        List<ProjectAward> pAward = (from s in db.ProjectAwards where s.ProjectId == model.SPtId select s).ToList();
                        if (pAward != null)
                        {
                            foreach (var item in pAward)
                            {
                                //delete them from ProjectAward table
                                ProjectAward PAward = db.ProjectAwards.Find(item.Id);
                                db.Entry(PAward).State = EntityState.Deleted;
                                db.SaveChanges();
                                //delete them from Award table

                                Award Award = db.Awards.Find(item.AwardId);
                                var itemm = item.AwardId;
                                db.Entry(Award).State = EntityState.Deleted;
                                db.SaveChanges();

                            }
                        }
                        // Now I will add every award again to award table and mapping table
                        for (int i = 0; i < model.AwardsList.Count; i++)
                        {
                            Award award = new Award();

                            award.awardName = model.AwardsList.ElementAt(i).Trim();

                            using (Entities entity = new Entities())
                            {
                                entity.Awards.Add(award);
                                entity.SaveChanges();
                            }
                            //get last added award id 
                            Award LastAward = db.Awards.OrderByDescending(o => o.awardId).FirstOrDefault();
                            ProjectAward projectAward = new ProjectAward();
                            projectAward.AwardId = LastAward.awardId;
                            projectAward.ProjectId = model.SPtId;
                            using (Entities entity = new Entities())
                            {
                                entity.ProjectAwards.Add(projectAward);
                                entity.SaveChanges();
                            }
                        }

                    }
                }

                ///////////////programming languages//////////////////////////////////////////////////
                string PL = "";
                //set programming languages in one string 
                if (model.PLList != null)
                {
                    if (model.PLList.Count > 0)
                    {
                        for (int i = 0; i < model.PLList.Count - 1; i++)
                        {
                            if (model.PLList.ElementAt(i).Trim() != "")
                                PL += model.PLList.ElementAt(i).Trim().ToLower() + ",";

                        }
                        if (model.PLList.ElementAt(model.PLList.Count - 1).Trim() != "")
                            PL += model.PLList.ElementAt(model.PLList.Count - 1).Trim().ToLower();
                    }
                }

                ProjectEdit(model.SPtId, model.SPName, model.SPVideos, model.SPAbstract, model.SPGrade, PL, model.Section, model.Year, File);
                return new JsonResult
                {
                    Data = "Saved successfully"
                };

            }
            catch (Exception ex)
            {
                return new JsonResult
                {
                    Data = "an error occurred " + ex.Message
                };
            }
        }

        private void ProjectEdit(int SPtId, string SPName, string SPVideos, string SPAbstract, string SPGrade, string PL, string Section, short? year, HttpPostedFileBase file)
        {
            SeniorProject project = db.SeniorProjects.Find(SPtId);

            project.SPtId = SPtId;
            project.SPName = SPName;
            project.SPAbstract = SPAbstract;
            project.SPVideos = SPVideos;
            project.SPGrade = SPGrade;
            project.progLang = PL;
            project.Section = Section;
            project.Year = year;

            if (file != null && file.ContentLength > 0)
            {
                DateTime now = DateTime.Now;
                if (Path.GetExtension(file.FileName).ToUpper() == ".PDF")
                {
                    // Get file data
                    byte[] data = new byte[] { };
                    using (var binaryReader = new BinaryReader(file.InputStream))
                    {
                        data = binaryReader.ReadBytes(file.ContentLength);
                    }

                    project.SPReport = data;
                }
            }





            db.Entry(project).State = EntityState.Modified;
            db.SaveChanges();
        }

        // GET: SeniorProjects/Delete/5
        public ActionResult Delete(int? id)
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
            return View(seniorProject);
        }

        // POST: SeniorProjects/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            SeniorProject seniorProject = db.SeniorProjects.Find(id);
            db.SeniorProjects.Remove(seniorProject);
            db.SaveChanges();
            return RedirectToAction("Projects", "SeniorProjectt");
        }
    }


}

