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
    public class LoginRequiredModel : PageModel
    {
       private readonly SignInManager<ApplicationUser> _signInManager;
       private readonly ISessionCookieService _sessionCookieService;
       private readonly ILogger<LoginRequiredModel> _logger;

        public LoginRequiredModel   (   SignInManager<ApplicationUser> signInManager, 
                                        ISessionCookieService sessionCookieSvc,
                                        ILogger<LoginRequiredModel> logger
                                    )
        {
            _signInManager = signInManager;
            _sessionCookieService = sessionCookieSvc;
            _logger = logger;
        }

        public void OnGet()
        {
            System.Console.WriteLine("[LoginRequiredModel][OnGet]");
        }

        public async Task<IActionResult> OnPost(string returnUrl = null)
        {
            ApplicationUser appUser = null;

            if ( _signInManager != null && _signInManager.UserManager != null )
            {
                appUser = await _signInManager.UserManager.GetUserAsync(HttpContext.User);
                if ( appUser != null ) 
                {
                    _logger.LogInformation("[LoginRequiredModel][OnPost] => Signing out '" + appUser.UserName + "'");
                }

                _logger.LogInformation("[LoginRequiredModel][OnPost] => Calling _signInManager.SignOutAsync()");

                await _signInManager.SignOutAsync();
            }
            
            _logger.LogInformation("[LoginRequiredModel][OnPost] => Deleting all cookies");
            
            foreach (var cookieKey in Request.Cookies.Keys)
            {
                Response.Cookies.Delete(cookieKey);
            }
            
            _logger.LogInformation("[LoginRequiredModel][OnPost] => Deleting Database SessionCookie");
            
            if ( _sessionCookieService != null && appUser != null )
            {
                 _sessionCookieService.RemoveSessionCookie(appUser.UserName);
            }

            return Page();
        }
    }
}