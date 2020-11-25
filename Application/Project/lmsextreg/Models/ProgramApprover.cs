using System.ComponentModel.DataAnnotations;
using lmsextreg.Data;

namespace lmsextreg.Models
{
    public class ProgramApprover
    {
        [Required]
        public int LMSProgramID { get; set; }

        ////////////////////////////////////////////////////////////
        // ApproverUserId:
        // Same value as the 'Id' column in the 'AspNetUser' table
        ///////////////////////////////////////////////////////////
        [Required]
        public string ApproverUserId { get; set; }

        [Required]
        public bool EmailNotify { get; set; } = true;

        ///////////////////////////////////////////////////////////
        // Navigation Properties
        ///////////////////////////////////////////////////////////        
        public LMSProgram LMSProgram { get; set; }
         public ApplicationUser Approver { get; set; }        
    }
}