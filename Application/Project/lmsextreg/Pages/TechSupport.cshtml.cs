using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;

namespace lmsextreg.Pages
{
    [AllowAnonymous] 
    public class TechSupportModel : PageModel
    {
        public string Message { get; set; }

        public void OnGet()
        {
            //Message = "Your contact page.";
        }
    }
}
