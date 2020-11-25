using System;
using System.ComponentModel.DataAnnotations;
using lmsextreg.Data;

namespace lmsextreg.Models
{
    public class EnrollmentHistory
    {
        ////////////////////////////////////////////////////////////
        // EnrollmenHistoryID:
        // Primary Key
        ///////////////////////////////////////////////////////////
        [Required]
        public int EnrollmentHistoryID { get; set; }
        
        ////////////////////////////////////////////////////////////
        // ProgramEnrollmentID:
        // Foreign-key reference to ProgramEnrollment table
        ///////////////////////////////////////////////////////////
        [Required]
        public int ProgramEnrollmentID { get; set; }
      
        ////////////////////////////////////////////////////////////
        // StatusTransitionID:
        // Foreign-key reference to StatusTransition table
        ///////////////////////////////////////////////////////////      
        [Required]
        public int StatusTransitionID { get; set; }

        /////////////////////////////////////////////////////////////
        // ActorUserId:
        // (Same value as the 'Id' column in the 'AspNetUser' table)
        ////////////////////////////////////////////////////////////
        [Required]        
        public string ActorUserId { get; set; }

        public string ActorRemarks { get; set; }

        ////////////////////////////////////////////////////////////
        // DateCreated:
        // Date that row was originally inserted
        ///////////////////////////////////////////////////////////
        [Required]
         public DateTime DateCreated { get; set; }

        ///////////////////////////////////////////////
        // Navigation Properties
        ///////////////////////////////////////////////        
        //public ProgramEnrollment ProgramEnrollment {get; set; }
    
        public StatusTransition StatusTransition { get; set; }  

        public ApplicationUser Actor {get; set; }
    }
}