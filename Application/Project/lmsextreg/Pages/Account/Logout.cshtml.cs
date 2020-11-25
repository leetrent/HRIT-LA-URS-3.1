using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using lmsextreg.Data;
using lmsextreg.Services;

namespace lmsextreg.Pages.Account
{
    [AllowAnonymous]
    public class LogoutModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;

        private readonly ISessionCookieService _sessionCookieService;
        private readonly ILogger<LogoutModel> _logger;

        public LogoutModel( SignInManager<ApplicationUser> signInManager, 
                            ISessionCookieService sessionCookieSvc,
                            ILogger<LogoutModel> logger)
        {
            _signInManager = signInManager;
            _logger = logger;
            _sessionCookieService = sessionCookieSvc;
        }

        public void OnGet()
        {
            System.Console.WriteLine("[LogoutModel][OnGet]");
        }

        /*
            SignOutAsync clears the user's claims stored in a cookie. 
            Don't redirect after calling SignOutAsync or the user will not be signed out.
            https://docs.microsoft.com/en-us/aspnet/core/security/authentication/identity?view=aspnetcore-2.2&tabs=visual-studio#scaffold-register-login-and-logout
        */
        /*
            BUG:LogOut: Still signed in after SignOutAsync!   
            https://github.com/aspnet/AspNetCore.Docs/issues/6439      
        */
        // public async Task<IActionResult> OnPost(string returnUrl = null)
        // {
        //     ApplicationUser appUser = await _signInManager.UserManager.GetUserAsync(HttpContext.User);
             
        //      _logger.LogInformation("[LogoutModel][OnPost] => Signing out '" + appUser.UserName + "'");

        //     await _signInManager.SignOutAsync();
        //     foreach (var cookieKey in Request.Cookies.Keys)
        //     {
        //         Response.Cookies.Delete(cookieKey);
        //     }

        //     _sessionCookieService.RemoveSessionCookie(appUser.UserName);

        //     return Page();
        // }

        public async Task<IActionResult> OnPost(string returnUrl = null)
        {
            ApplicationUser appUser = null;

            if ( _signInManager != null && _signInManager.UserManager != null )
            {
                appUser = await _signInManager.UserManager.GetUserAsync(HttpContext.User);
                if ( appUser != null ) 
                {
                    _logger.LogInformation("[LogoutModel][OnPost] => Signing out '" + appUser.UserName + "'");
                }
                await _signInManager.SignOutAsync();
            }
            
            foreach (var cookieKey in Request.Cookies.Keys)
            {
                Response.Cookies.Delete(cookieKey);
            }

            if ( _sessionCookieService != null && appUser != null )
            {
                 _sessionCookieService.RemoveSessionCookie(appUser.UserName);
            }

            return Page();
        }
    }
}