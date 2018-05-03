using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SaVeIT_Final.Models
{
    public class User2
    {
        [DisplayName("User ID")]
        [Required(ErrorMessage = "User ID is required.")]
        public string userId { get; set; }

        public string userName { get; set; }
        [DataType(DataType.EmailAddress)]
        [Required(ErrorMessage = "Email is required.")]
        public string userEmail { get; set; }
        public Nullable<int> userRole { get; set; }
        public Nullable<int> projectGroup { get; set; }

        [DisplayName("Password")]
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Password is required.")]
        public string password { get; set; }

        public string LoginErrorMessage { get; set; }
    }
}