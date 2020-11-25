using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using lmsextreg.Data;

namespace lmsextreg.Pages.Account
{
    [AllowAnonymous]
    public class Logout2Model : PageModel
    {
        public void OnGet()
        {
            System.Console.WriteLine("[Logout2Model][OnGet]");
        }
    }
}