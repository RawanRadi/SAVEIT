using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SaVeIT_Final.Models
{
    public class AreaOfInterest2
    {
        public int AOIId { get; set; }
        [Required(ErrorMessage = "Area of interest name is required")]
        public string AOIName { get; set; }
        public byte[] AOIIcon { get; set; }
    }
}