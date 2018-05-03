//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SaVeIT_Final.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class SeniorProject
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public SeniorProject()
        {
            this.ProjectAwards = new HashSet<ProjectAward>();
            this.ProjectsAOIs = new HashSet<ProjectsAOI>();
            this.Stu_Projects = new HashSet<Stu_Projects>();
            this.Students = new HashSet<Student>();
            this.Users = new HashSet<User>();
        }
    
        public int SPtId { get; set; }
        public string SPName { get; set; }
        public string SPAbstract { get; set; }
        public string SPGrade { get; set; }
        public byte[] SPReport { get; set; }
        public string SPVideos { get; set; }
        public string supervisorId { get; set; }
        public string supervisorUserId { get; set; }
        public Nullable<int> awardId { get; set; }
        public string progLang { get; set; }
        public string SPReportName { get; set; }
        public Nullable<short> Year { get; set; }
        public string Section { get; set; }
    
        public virtual Award Award { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ProjectAward> ProjectAwards { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ProjectsAOI> ProjectsAOIs { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Stu_Projects> Stu_Projects { get; set; }
        public virtual Supervisor Supervisor { get; set; }
        public virtual User User { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Student> Students { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<User> Users { get; set; }
    }
}