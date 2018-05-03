using System.Collections.Generic;
namespace SaVeIT_Final.Models
{
    public class supervisor1
    {
        public string userId { get; set; }
        public string userName { get; set; }
        public string userEmail { get; set; }
        public virtual ICollection<SeniorProject> SeniorProjects { get; set; }
        public virtual ICollection<AreaOFInterest> AOIs { get; set; }
    }
}