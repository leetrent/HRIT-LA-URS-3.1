using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;

namespace lmsextreg.Pages
{
   [AllowAnonymous] 
    public class AboutModel : PageModel
    {
        public string Message { get; set; }

        public void OnGet()
        {
            // Message = "Your application description page.";
            Message = "GSA Learning Academy - User Registration";
        }
    }
}
