using System;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace lmsextreg.Models
{
    public class SessionCookie
    {
        [Required]
        public string UserName { get; set; }   
        
        [Required]
        public string CookieName { get; set; }   

        [Required]
        public string CookieValue { get; set; }           

        [Required]
        public DateTime LastAccessedOn { get; set; }

        public override String ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("SessionCookie = {");
            sb.Append("UserName: ");
            sb.Append(this.UserName);
            sb.Append(", CookieName: ");
            sb.Append(this.CookieName);                       
            sb.Append(", LastAccessedOn: ");
            sb.Append(this.LastAccessedOn);
            sb.Append("; CookieValue: ");
            sb.Append(this.CookieValue);  
            sb.Append("}");                                      

            return sb.ToString();
        } 

    }
}