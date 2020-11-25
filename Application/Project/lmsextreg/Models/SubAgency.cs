using System;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace lmsextreg.Models
{
   public class SubAgency
   {
      [DatabaseGenerated(DatabaseGeneratedOption.None)]
      [Required]        
      public string SubAgencyID { get; set; }
      [Required]
      public string AgencyID { get; set; }
      [Required]
      public int DisplayOrder { get; set; }
      public string OPMCode{ get; set; }           
      [Required]
      public string SubAgencyName { get; set; }        
      public string TreasuryCode{ get; set; }

      ////////////////////////////////////
      // Navigation property 
      ////////////////////////////////////
      public Agency Agency { get; set; }        
   }
}