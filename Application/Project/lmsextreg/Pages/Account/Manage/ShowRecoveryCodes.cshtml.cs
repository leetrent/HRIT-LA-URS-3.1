using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity;
using lmsextreg.Constants;
using lmsextreg.Data;
using lmsextreg.Services;

namespace lmsextreg.Pages.Account.Manage
{
    public class ShowRecoveryCodesModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEventLogService _eventLogService;

        public ShowRecoveryCodesModel
        (
            UserManager<ApplicationUser> userManager,
            IEventLogService eventLogSvc
        )
        {
            _userManager        = userManager;
            _eventLogService    = eventLogSvc;
        }

        public string[] RecoveryCodes { get; private set; }

        public IActionResult OnGet()
        {
            RecoveryCodes = (string[])TempData["RecoveryCodes"];
            if (RecoveryCodes == null)
            {
                return RedirectToPage("TwoFactorAuthentication");
            }
  
            var user = _userManager.GetUserAsync(User).Result;
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }            
            ///////////////////////////////////////////////////////////////////
            // Log the 'SHOW_RECOVERY_CODES' event
            ///////////////////////////////////////////////////////////////////
            _eventLogService.LogEvent(EventTypeCodeConstants.SHOW_RECOVERY_CODES, user);                  

            return Page();
        }
    }
}