using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using lmsextreg.Services;
using lmsextreg.Data;
using lmsextreg.ApiModels;
using lmsextreg.Constants;

namespace lmsextreg.Controllers
{
    [Route("api/user/twofactorauth")]
    [ApiController]
    [Authorize(Roles = "ADMIN")]

    public class UserTwoFactorAuthApiController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserService _userService;
        private readonly IEventLogService _eventLogService;

        public UserTwoFactorAuthApiController(UserManager<ApplicationUser> userMgr, IUserService userSvc, IEventLogService eventLogSvc)
        {
            _userManager = userMgr;
            _userService = userSvc;
            _eventLogService = eventLogSvc;
        }

        [HttpGet]
        public ActionResult<bool> IsTwoFactorAuthEnabled(string userId)
        {
            string logSnippet = new StringBuilder("[")
                                .Append(DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"))
                                .Append("][UserTwoFactorAuthApiController][IsTwoFactorAuthEnabled][HttpGet] => ")
                                .ToString();

            Console.WriteLine(logSnippet + $"(userId).................: '{userId}'");
            ApplicationUser appUser = _userService.RetrieveUserByUserId(userId);
            Console.WriteLine(logSnippet + $"(appUser == null)........: '{appUser == null}'");
            Console.WriteLine(logSnippet + $"(appUser.TwoFactorEnabled).: '{appUser.TwoFactorEnabled}'");
            return (appUser.TwoFactorEnabled == true);
        }

        [HttpPost]
        public ActionResult<string> DisableTwoFactorAuth(UserAccount userAccount)
        {
            string logSnippet = new StringBuilder("[")
                                .Append(DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"))
                                .Append("][UserTwoFactorAuthApiController][DisableTwoFactorAuth][HttpPost] => ")
                                .ToString();

            Console.WriteLine(logSnippet + $"(userAccount == null): '{userAccount == null}'");
            Console.WriteLine(logSnippet + $"(userAccount.Id).....: '{userAccount.Id}'");
            int rowsUpdated = _userService.DisableTwoFactorAuth(userAccount.Id);
            Console.WriteLine(logSnippet + $"(rowsUpdated)........: '{rowsUpdated}'");

            ApplicationUser admin = this.GetCurrentUserAsync().Result;
            ApplicationUser user = _userService.RetrieveUserByUserId(userAccount.Id);

            Console.WriteLine(logSnippet + $"(admin == null): '{admin == null}'");
            Console.WriteLine(logSnippet + $"(user == null).: '{user == null}'");

            UserAdminEvent uaEvent = new UserAdminEvent(EventTypeCodeConstants.ADMIN_DISABLED_TWO_FACTOR_AUTH, admin, user);
            _eventLogService.LogEvent(uaEvent);

            return RedirectToAction(nameof(IsTwoFactorAuthEnabled), new { @userId = userAccount.Id });
        }

        private Task<ApplicationUser> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);
    }
}