
using System;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace lmsextreg.Models
{
    public class EnrollmentStatus
    {
        [Key]
        [Required]
        [Display(Name = "Status Code")]
        public string StatusCode { get; set; }
        [Required]
         [Display(Name = "Status")]
        public string StatusLabel { get; set; }
        public ushort DisplayOrder { get; set; }

        public override String ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("[");
            sb.Append("StatusCode=");
            sb.Append(this.StatusCode);
            sb.Append(";StatusLabel=");
            sb.Append(this.StatusLabel);
            sb.Append("]");                                      

            return sb.ToString();
        }        
    }
}