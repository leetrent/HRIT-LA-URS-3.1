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
    public class LoginWith2fa2Model : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;

        private readonly ISessionCookieService _sessionCookieService;
        private readonly ILogger<LoginWith2fa2Model> _logger;

         public LoginWith2fa2Model( UserManager<ApplicationUser> userMgr, 
                                    ISessionCookieService sessionCookieSvc,
                                    ILogger<LoginWith2fa2Model> logger )
        {
            _userManager = userMgr;
            _sessionCookieService   = sessionCookieSvc;
            _logger                 = logger;
        }   

        public void OnGet()
        {
            _logger.LogDebug("\n[LoginWith2fa2Model][OnGet] =>");
        
            ApplicationUser appUser =  _userManager.GetUserAsync(HttpContext.User).Result;

            _sessionCookieService.RecordSessionCookie(appUser.UserName, Request.Cookies);
   
             _logger.LogDebug("\n<= [LoginWith2fa2Model][OnGet]");       
        }
    }
}