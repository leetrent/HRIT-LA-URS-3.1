using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using lmsextreg.Models;

namespace lmsextreg.Data
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        [Required]
        public string FirstName { get; set; }
        
        public string MiddleName { get; set; }
        
        [Required]
        public string LastName { get; set; }

        [Required]
        public string JobTitle { get; set; }   
        
        [Required]
        public string AgencyID { get; set; }
                
        [Required]
        public string SubAgencyID { get; set; }
              
        [Required]
        public DateTime DateRegistered { get; set; }
        
        [Required]
        public DateTime DateAccountExpires { get; set; }
        
        [Required]
        public DateTime DatePasswordExpires { get; set; }

        [Required]
        public bool RulesOfBehaviorAgreedTo { get; set; }

        ///////////////////////////////////
        // Full Name: Derived Value
        ///////////////////////////////////        
        [Display(Name = "Name")]
        public string FullName
        {
            get
            {
                return LastName + ", " + FirstName;
            }
        }

        public string FirstMiddleLastName
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(this.FirstName);
                sb.Append(" ");
                if ( String.IsNullOrEmpty(this.MiddleName) 
                        || String.IsNullOrWhiteSpace(this.MiddleName) )
                {
                    sb.Append(this.MiddleName);
                    sb.Append(" ");
                }
                sb.Append(this.LastName);
                return sb.ToString();
            }
        }

        public bool IsLockedOut
        {
            get
            {
                return (this.LockoutEnd != null && this.LockoutEnd > DateTime.Now);
            }
        }

        ///////////////////////////////////
        // Navigation Property
        ///////////////////////////////////        
        public Agency Agency { get; set; }
        
        ///////////////////////////////////
        // Navigation Property
        ///////////////////////////////////        
        public SubAgency SubAgency { get; set; }

        public override String ToString()
        {
            StringBuilder sb = new StringBuilder();
          
            sb.Append("[");
            sb.Append("UserName=");
            sb.Append(this.UserName);
            sb.Append("; Email=");
            sb.Append(this.Email);
            sb.Append("; LastName=");
            sb.Append(this.LastName);   
            sb.Append("; FirstName=");
            sb.Append(this.FirstName);     
            sb.Append("; MiddleName=");
            sb.Append(this.MiddleName);           
            sb.Append("; AgencyID=");
            sb.Append(this.AgencyID);         
            sb.Append("; SubAgencyID=");
            sb.Append(this.SubAgencyID);   
            sb.Append("]");                                      

            return sb.ToString();
        }        

        public String ToEventLog()
        {
            StringBuilder sb = new StringBuilder();
          
            sb.Append("[");
            sb.Append("UserName=");
            sb.Append(this.UserName);
            sb.Append("; Email=");
            sb.Append(this.Email);
            sb.Append("; LastName=");
            sb.Append(this.LastName);   
            sb.Append("; FirstName=");
            sb.Append(this.FirstName);     
            sb.Append("; MiddleName=");
            sb.Append(this.MiddleName);           
           
            if (this.Agency == null)
            {
                sb.Append("; AgencyID=");
                sb.Append(this.AgencyID);  
            }
            else
            {
                sb.Append("; Agency=");
                sb.Append(this.Agency.AgencyName);   
            }
      
            if (this.SubAgency == null)
            {
                sb.Append("; SubAgencyID=");
                sb.Append(this.SubAgencyID);   
            }
            else
            {
                sb.Append("; SubAgency=");
                sb.Append(this.SubAgency.SubAgencyName);              
            }
 
            sb.Append("]");                                      

            return sb.ToString();            
        }
    }
}
