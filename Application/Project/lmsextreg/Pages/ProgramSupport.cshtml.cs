using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;

namespace lmsextreg.Pages
{
    [AllowAnonymous] 
    public class ProgramSupportModel : PageModel
    {
        public string Message { get; set; }

        public void OnGet()
        {
            //Message = "Your contact page.";
        }
    }
}
