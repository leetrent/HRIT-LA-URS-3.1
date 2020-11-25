using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using lmsextreg.Constants;
using lmsextreg.Data;
using lmsextreg.Services;
using lmsextreg.Repositories;
using lmsextreg.Models;

namespace lmsextreg.Pages.Account
{
    [AllowAnonymous]
    public class ConfirmEmailModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEventLogService _eventLogService;
        private readonly ILogger<ConfirmEmailModel> _logger;
        private readonly IConfirmEmailService _confirmEmailService;

        public ConfirmEmailModel
        (
            UserManager<ApplicationUser> userManager,
            IEventLogService eventLogSvc,
            ILogger<ConfirmEmailModel> logger,
            IConfirmEmailService confirmEmailSvc
        )
        {
            _userManager = userManager;
            _eventLogService = eventLogSvc;
            _logger = logger;
            _confirmEmailService = confirmEmailSvc;
        }

        public async Task<IActionResult> OnGetAsync(string userId, string code)
        {
            Console.WriteLine("[" + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + "][ConfirmEmailModel][OnGetAsync] => userId: '" + userId + "'");
            Console.WriteLine("[" + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + "][ConfirmEmailModel][OnGetAsync] => code..: '" + code + "'");
            
            ////////////////////////////////////////////////////////////////////////////////
            // Validate passed-in parameter: userId
            ////////////////////////////////////////////////////////////////////////////////
            if (userId == null )
            {
                Console.WriteLine("[" + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") 
                                    + "][ConfirmEmailModel][OnGetAsync] => userId is NULL - redirecting to Index page");
                return RedirectToPage("./ConfirmEmailFailure");
            }

            ////////////////////////////////////////////////////////////////////////////////
            // Validate passed-in parameter: code
            ////////////////////////////////////////////////////////////////////////////////
            if (code == null)
            {
                Console.WriteLine("[" + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") 
                                    + "][ConfirmEmailModel][OnGetAsync] => code for '"
                                    + userId + "' is NULL - redirecting to ConfirmEmailFailure page");

               return RedirectToPage("./ConfirmEmailFailure");
            }

            Console.WriteLine("[" + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") 
                                  + "][ConfirmEmailModel][OnGetAsync] => Email Verification Code for '"
                                  + userId + "' has a length of " + code.Length + " characters.");

            if (code.Length > MiscConstants.GUID_LENGTH)
            {
                Console.WriteLine("[" + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") 
                                    + "][ConfirmEmailModel][OnGetAsync][" + userId + "] => Truncating email verifcation code because it has a length of " + code.Length + " characters.");
                Console.WriteLine("[" + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") 
                                    + "][ConfirmEmailModel][OnGetAsync][" + userId + "] => OLD LENGTH(email verification code): " + code.Length + " characters.");
                code = code.Substring(0, MiscConstants.GUID_LENGTH);
                Console.WriteLine("[" + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") 
                                    + "][ConfirmEmailModel][OnGetAsync][" + userId + "] => NEW LENGTH(email verification code): " + code.Length + " characters.");          
            }

            ////////////////////////////////////////////////////////////////////////////////
            // Find ApplicationUser for passsed-in parameter: userId
            ////////////////////////////////////////////////////////////////////////////////
            var user = await _userManager.FindByIdAsync(userId);

            Console.WriteLine("[" + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") 
                                + "][ConfirmEmailModel][OnGetAsync] => ApplicationUser IS NULL: " 
                                + (user == null));

            ////////////////////////////////////////////////////////////////////////////////
            // If ApplicationUser IS NOT FOUND, redirect to ConfirmEmailFailure page
            ////////////////////////////////////////////////////////////////////////////////
            if (user == null)
            {
                Console.WriteLine("[" + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") 
                                    + "][ConfirmEmailModel][OnGetAsync] => ApplicationUser with userId of '" 
                                    + userId + "' NOT FOUND - redirecting to ConfirmEmailFailure page.");

                 return RedirectToPage("./ConfirmEmailFailure");
            }

            ////////////////////////////////////////////////////////////////////////////////
            // ApplicationUser WAS FOUND, validate email verification token ...
            ////////////////////////////////////////////////////////////////////////////////

            Console.WriteLine("[" + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + "][ConfirmEmailModel][OnGetAsync] => ApplicationUser.ToString(): " 
                                + (user.ToString()));
            Console.WriteLine("[" + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + "][ConfirmEmailModel][OnGetAsync] => ApplicationUser.ToEventLog(): " 
                                + (user.ToEventLog()));

            if ( _confirmEmailService.IsConfirmed(user, code) )
            {
                Console.WriteLine("[" + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") 
                    + "][ConfirmEmailModel][OnGetAsync][" + userId + "] => EMAIL CONFIRMED!"); 

                ///////////////////////////////////////////////////////////////////
                // Log the 'EMAIL_CONFIRMED' event
                ///////////////////////////////////////////////////////////////////
                _eventLogService.LogEvent(EventTypeCodeConstants.EMAIL_CONFIRMED, user);             

                return Page();
            }
            else
            {
                return RedirectToPage("./ConfirmEmailFailure");
            }
        }
    }
}
