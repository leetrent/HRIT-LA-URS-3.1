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
    [Route("api/user/account/lock")]
    [ApiController]
    [Authorize(Roles = "ADMIN")]

    public class UserAccountLockApiController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserService _userService;
        private readonly IEventLogService _eventLogService;

        public UserAccountLockApiController(UserManager<ApplicationUser> userMgr, IUserService userSvc, IEventLogService eventLogSvc)
        {
            _userManager        = userMgr;
            _userService        = userSvc;
            _eventLogService    = eventLogSvc;
        }

        [HttpGet]
        public ActionResult<bool> IsUserAccountLocked(string userId)
        {
            string logSnippet = new StringBuilder("[")
                                .Append(DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"))
                                .Append("][UserAccountLockApiController][IsUserAccountLocked][HttpGet] => ")
                                .ToString();

            Console.WriteLine(logSnippet + $"(userId)....................: '{userId}'");
            ApplicationUser appUser = _userService.RetrieveUserByUserId(userId);
            Console.WriteLine(logSnippet + $"(appUser == null)...........: '{appUser == null}'");
            Console.WriteLine(logSnippet + $"(appUser.LockoutEnd == null): '{appUser.LockoutEnd == null}'");
            Console.WriteLine(logSnippet + $"(appUser.LockoutEnd)........: '{appUser.LockoutEnd}'");
            return (appUser.LockoutEnd != null);
        }

        [HttpPost]
        public ActionResult<string> UnlockUserAccount(UserAccount userAccount)
        {
            string logSnippet = new StringBuilder("[")
                                .Append(DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"))
                                .Append("][UserAccountLockApiController][UnlockUserAccount][HttpPost] => ")
                                .ToString();

            _userService.UnlockUser(userAccount.Id);

            ApplicationUser admin = this.GetCurrentUserAsync().Result;
            ApplicationUser user = _userService.RetrieveUserByUserId(userAccount.Id);

            Console.WriteLine(logSnippet + $"(admin == null): '{admin == null}'");
            Console.WriteLine(logSnippet + $"(user == null).: '{user == null}'");

            UserAdminEvent uaEvent = new UserAdminEvent(EventTypeCodeConstants.ADMIN_UNLOCKED_ACCOUNT, admin, user);
            _eventLogService.LogEvent(uaEvent);

            return RedirectToAction(nameof(IsUserAccountLocked), new { @userId = userAccount.Id });
        }

        private Task<ApplicationUser> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);
    }


}