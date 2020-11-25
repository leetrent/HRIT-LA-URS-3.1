using System.Text;using System;

using System.Collections.Generic;

using System.ComponentModel.DataAnnotations;

namespace lmsextreg.Models
{
    public class LMSProgram
    { 
        [Required]
        public int LMSProgramID { get; set; }
        [Required]
        [Display(Name = "Program Code")]
        public string ShortName { get; set; }
        [Required]
        [Display(Name = "Program Name")]
        public string LongName { get; set; }
        public string CommonInbox { get; set; }

        public int ExpiryDays {get; set; }
        public ICollection<ProgramApprover> ProgramApprovers { get; set; }

        public override String ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("LMSProgram = {");
            sb.Append("LMSProgramID=");
            sb.Append(this.LMSProgramID);
            sb.Append(";ShortName=");
            sb.Append(this.ShortName);
            sb.Append(this.LongName);   
            sb.Append(";LastName=");
            sb.Append(this.LongName);   
            sb.Append(";CommonInbox=");
            sb.Append(this.CommonInbox);    
            sb.Append(";ExpiryDays=");
            sb.Append(this.ExpiryDays);   
            sb.Append("}");                                      

            return sb.ToString();
        }         
     }
}