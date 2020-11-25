using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using lmsextreg.Data;
using lmsextreg.Constants;
using lmsextreg.Services;

namespace lmsextreg.Pages.Account.Manage
{
    public class ChangePasswordModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<ChangePasswordModel> _logger;
        private readonly IEventLogService _eventLogService;

        public ChangePasswordModel
        (
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ILogger<ChangePasswordModel> logger,
            IEventLogService eventLogSvc
        )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _eventLogService = eventLogSvc;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        public class InputModel
        {
            [Required]
            [DataType(DataType.Password)]
            [Display(Name = "Current password")]
            public string OldPassword { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "New password")]
            public string NewPassword { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm new password")]
            [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }
        }

        public async Task<IActionResult> OnGetAsync()
        {
            Console.WriteLine("[ChangePassword][OnGetAsync]: BEGIN");

            var user = await _userManager.GetUserAsync(User);
            Console.WriteLine("[ChangePassword][OnGetAsync]: \n User: " + user);

            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var hasPassword = await _userManager.HasPasswordAsync(user);
            Console.WriteLine("[ChangePassword][OnGetAsync]: hasPassword: " + hasPassword);

            if (!hasPassword)
            {
                Console.WriteLine("[ChangePassword][OnGetAsync]: Redirecting to ./SetPassword");
                return RedirectToPage("./SetPassword");
            }

            Console.WriteLine("[ChangePassword][OnGetAsync]: Rendering ChangePassword page");
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            Console.WriteLine("[ChangePassword][OnPostAsync]: BEGIN");

            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                Console.WriteLine("[ChangePassword][OnPostAsync]: userManager.GetUserAsync(User) returned NULL");
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }
           
            Console.WriteLine("[ChangePassword][OnPostAsync]: User: " + user);

            var changePasswordResult = await _userManager.ChangePasswordAsync(user, Input.OldPassword, Input.NewPassword);
            Console.WriteLine("[ChangePassword][OnPostAsync]: changePasswordResult.Succeeded: " + changePasswordResult.Succeeded);

            if (!changePasswordResult.Succeeded)
            {
                foreach (var error in changePasswordResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return Page();
            }

            await _signInManager.SignInAsync(user, isPersistent: false);
            _logger.LogInformation("User changed their password successfully.");
            StatusMessage = "Your password has been changed.";

            ///////////////////////////////////////////////////////////////////
            // Log the 'CHANGED_PASSWORD' event
            ///////////////////////////////////////////////////////////////////
            _eventLogService.LogEvent(EventTypeCodeConstants.CHANGED_PASSWORD, user); 

            /////////////////////////////////////////////////////////////
            // Now that password had been successfully changed,
            // update the ApplicationUser.DatePasswordExpires property 
            // to NOW + AccountConstants.DAYS_PASSWORD_EXPIRES
            ////////////////////////////////////////////////////////////
            user.DatePasswordExpires = DateTime.Now.AddDays(AccountConstants.DAYS_PASSWORD_EXPIRES);
            IdentityResult updateDatePasswordExpiresResult = await _userManager.UpdateAsync(user);
            Console.WriteLine("[ChangePassword][OnPostAsync]: updateDatePasswordExpiresResult.Succeeded: " 
                                + updateDatePasswordExpiresResult.Succeeded);

            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////
            // Check to see if Two-Factor Authentication has been enabled.
            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////
            Console.WriteLine("[ChangePassword][OnPostAsync] - ApplicationUser.TwoFactorEnabled: " + user.TwoFactorEnabled);
            if ( user.TwoFactorEnabled == false)
            {
                Console.WriteLine("[ChangePassword][OnPostAsync] - Two-factor auth NOT enabled, redirecting to './Manage/EnableAuthenticator' page");
                return RedirectToPage("./EnableAuthenticator");
            }

            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////
            // If we get this far, password was changed successfully and Two-Factor Authentication has been enabled.
            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////
            return RedirectToPage();
        }
    }
}
