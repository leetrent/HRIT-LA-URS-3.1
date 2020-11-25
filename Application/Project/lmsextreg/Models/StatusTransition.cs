using System.ComponentModel.DataAnnotations;

namespace lmsextreg.Models
{
    public class StatusTransition
    {
        [Required]
        public int StatusTransitionID { get; set; }
        [Required]
        public string FromStatusCode { get; set; }
        public string ToStatusCode { get; set; }
        [Required]
        public string TransitionCode { get; set; }
        [Required]        
        public string TransitionLabel { get; set; }
    
        ///////////////////////////////////////////////
        // Navigation Properties
        ///////////////////////////////////////////////        
       public EnrollmentStatus FromStatus {get; set; }
        [Required]
        public EnrollmentStatus ToStatus { get; set; }        
    }
}