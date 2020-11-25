using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using lmsextreg.Data;
using Microsoft.AspNetCore.Authentication;
using lmsextreg.Services;


namespace lmsextreg.Pages.Account
{
    [AllowAnonymous]
    public class LoginPrepModel : PageModel
    {
       private readonly SignInManager<ApplicationUser> _signInManager;
       private readonly ISessionCookieService _sessionCookieService;
       private readonly ILogger<LoginPrepModel> _logger;

        public LoginPrepModel   (   SignInManager<ApplicationUser> signInManager, 
                                    ISessionCookieService sessionCookieSvc,
                                    ILogger<LoginPrepModel> logger
                                )
        {
            _signInManager = signInManager;
            _sessionCookieService = sessionCookieSvc;
            _logger = logger;
        }

        public void OnGet()
        {
            System.Console.WriteLine("[LoginPrepModel][OnGet]");
        }
    }
}