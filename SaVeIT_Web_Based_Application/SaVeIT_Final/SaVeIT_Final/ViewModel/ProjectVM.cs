using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;
using SaVeIT_Final.Models;

namespace SaVeIT_Final.ViewModel
{
    public class ProjectVM
    {
        //[Required]
        //public string supervisorId { get; set; }
        //[Required]
        //public string supervisorName { get; set; }

        [Required]
        public string userId { get; set; }
        [Required]

        public string userName { get; set; }

        public User supervisor { get; set; }
        // [Required]
        //   public List<Student> Students { get; set; }
        //
        [Required]
        public List<User> Users { get; set; }
        //
        public string Section { get; set; }

        public int SPtId { get; set; }
        [Required]
        public string SPName { get; set; }
        [Required]
        public string SPAbstract { get; set; }
        public string SPGrade { get; set; }
        [Required]
        //[DataType(DataType.Upload)]
        // public HttpPostedFileBase File { get; set; }
        public byte[] SPReport { get; set; }
        public string SPVideos { get; set; }
        public Nullable<int> AwardId { get; set; }
        public List<string> AwardsList { get; set; }
        public List<string> AwardsAddedList { get; set; }
        public List<string> PLList { get; set; }
        public List<string> AOIList { get; set; }
        public List<string> AOIAddedList { get; set; }

        public Nullable<short> Year { get; set; }
    }
}