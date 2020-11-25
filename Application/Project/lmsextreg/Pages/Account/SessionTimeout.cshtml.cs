using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using lmsextreg.Data;
using Microsoft.AspNetCore.Authentication;


namespace lmsextreg.Pages.Account
{
    [AllowAnonymous]
    public class SessionTimeoutModel : PageModel
    {
       private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<SessionTimeoutModel> _logger;

        public SessionTimeoutModel(SignInManager<ApplicationUser> signInManager, ILogger<SessionTimeoutModel> logger)
        {
            _signInManager = signInManager;
            _logger = logger;
        }

        public async Task OnGetAsync()
        {
            _logger.LogInformation("[SessionTimeoutModel][OnGet]=>");

            await HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
            await HttpContext.SignOutAsync(IdentityConstants.TwoFactorRememberMeScheme);
            await HttpContext.SignOutAsync(IdentityConstants.TwoFactorUserIdScheme);

            await _signInManager.SignOutAsync();

            foreach (var cookieKey in Request.Cookies.Keys)
            {
                // System.Console.WriteLine("[LogoutModel][OnPost] => Deleting: " + cookieKey);
                Response.Cookies.Delete(cookieKey);
            }
           
           _logger.LogInformation("<=[SessionTimeoutModel][OnGet]");
        }
    }
}