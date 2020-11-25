
using System;
using System.ComponentModel.DataAnnotations;

namespace lmsextreg.Models
{
    public class EventType
    {   
        ////////////////////////////////////////////////////////////
        // EventTypeCode:
        // Primary-key (auto-generated)
        ////////////////////////////////////////////////////////////        
        [Key]
        [Required]
        [Display(Name = "Event Type Code")]
        public string EventTypeCode { get; set; }
        
        [Required]
        [Display(Name = "Event Type")]
        public string EventTypeLabel { get; set; }      
    }
}