using System;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace lmsextreg.Models
{
    public class Agency
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Required]
        public string AgencyID { get; set; }

        [Required]
        [Display(Name = "Agency")]
        public string AgencyName { get; set; }

        [Required]
        public int DisplayOrder { get; set; }

        public string OPMCode{ get; set; }

        public string TreasuryCode{ get; set; }

        public ICollection<SubAgency> SubAgencies { get; set; }
    }
}