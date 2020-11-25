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
    [Route("api/user/emailaddress")]
    [ApiController]
    [Authorize(Roles = "ADMIN")]

    public class UserEmailAddressApiController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserService _userService;
        private readonly IEventLogService _eventLogService;

        public UserEmailAddressApiController(UserManager<ApplicationUser> userMgr, IUserService userSvc, IEventLogService eventLogSvc)
        {
            _userManager = userMgr;
            _userService = userSvc;
            _eventLogService = eventLogSvc;
        }

        [HttpGet]
        public ActionResult<bool> IsEmailAddressConfirmed(string userId)
        {
            string logSnippet = new StringBuilder("[")
                                .Append(DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"))
                                .Append("][UserEmailAddressApiController][IsEmailAddressConfirmed][HttpGet] => ")
                                .ToString();

            Console.WriteLine(logSnippet + $"(userId).................: '{userId}'");
            ApplicationUser appUser = _userService.RetrieveUserByUserId(userId);
            Console.WriteLine(logSnippet + $"(appUser == null)........: '{appUser == null}'");
            Console.WriteLine(logSnippet + $"(appUser.EmailConfirmed).: '{appUser.EmailConfirmed}'");
            return (appUser.EmailConfirmed == true);
        }

        [HttpPost]
        public ActionResult<string> ConfirmEmailAddress(UserAccount userAccount)
        {
            string logSnippet = new StringBuilder("[")
                                .Append(DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"))
                                .Append("][UserEmailAddressApiController][ConfirmEmailAddress][HttpPost] => ")
                                .ToString();

            Console.WriteLine(logSnippet + $"(userAccount == null): '{userAccount == null}'");
            Console.WriteLine(logSnippet + $"(userAccount.Id).....: '{userAccount.Id}'");
            int rowsUpdated = _userService.ConfirmEmailAddress(userAccount.Id);
            Console.WriteLine(logSnippet + $"(rowsUpdated)........: '{rowsUpdated}'");

            ApplicationUser admin = this.GetCurrentUserAsync().Result;
            ApplicationUser user = _userService.RetrieveUserByUserId(userAccount.Id);

            Console.WriteLine(logSnippet + $"(admin == null): '{admin == null}'");
            Console.WriteLine(logSnippet + $"(user == null).: '{user == null}'");

            UserAdminEvent uaEvent = new UserAdminEvent(EventTypeCodeConstants.ADMIN_CONFIRMED_EMAIL, admin, user);
            _eventLogService.LogEvent(uaEvent);

            return RedirectToAction(nameof(IsEmailAddressConfirmed), new { @userId = userAccount.Id });
        }

        private Task<ApplicationUser> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);
    }
}