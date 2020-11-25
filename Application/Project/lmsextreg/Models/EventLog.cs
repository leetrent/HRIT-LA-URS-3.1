using System;
using System.ComponentModel.DataAnnotations;
using lmsextreg.Data;

namespace lmsextreg.Models
{
    public class EventLog
    {
        ////////////////////////////////////////////////////////////
        // EventLogID:
        // Primary-key (auto-generated)
        ////////////////////////////////////////////////////////////
        [Required]
        public int EventLogID { get; set; }

        ////////////////////////////////////////////////////////////
        // EventTypeCode:
        // Foreign-key reference to EventType table
        ///////////////////////////////////////////////////////////
        [Required]
        public string EventTypeCode { get; set; }      

        ////////////////////////////////////////////////////////////
        // UserCreatedID:
        // User who inserted this row
        // (Same value as the 'Id' column in the 'AspNetUser' table)
        ////////////////////////////////////////////////////////////
        [Required]
        public string UserCreatedID { get; set; }

        ////////////////////////////////////////////////////////////////////
        // UserCreatedName:
        // User who inserted this row
        // (Same value as the 'UserName' column in the 'AspNetUser' table)
        ///////////////////////////////////////////////////////////////////
        [Required]
        public string UserCreatedName { get; set; }        

        ////////////////////////////////////////////////////////////////////
        // DataValues:
        // Data attributes that are specific to EventType
        ///////////////////////////////////////////////////////////////////
        public String DataValues {get; set;}
              
        ////////////////////////////////////////////////////////////
        // DateCreated:
        // Date that row was originally inserted
        ///////////////////////////////////////////////////////////
        [Required]
        public DateTime DateTimeCreated { get; set; }    

        ///////////////////////////////////////////////   
        // Navigation Property:
        ///////////////////////////////////////////////   
        // EventType:
        // Navigation property to EventType entity
        ///////////////////////////////////////////////
         public EventType EventType { get; set; }     

        /////////////////////////////////////////////////   
        // Navigation Property:
        /////////////////////////////////////////////////   
        // UserCreated:
        // Navigation property to ApplicationUser entity
        ////////////////////////////////////////////////
        public ApplicationUser UserCreated { get; set; }                       
    }
}
