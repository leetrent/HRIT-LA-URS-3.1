using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Text;
using System.Text.Encodings.Web;
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

namespace lmsextreg.Pages.Account.Manage
{
    [Authorize(Roles = "STUDENT,APPROVER,ADMIN")]
    public class EnableAuthenticatorModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<EnableAuthenticatorModel> _logger;
        private readonly UrlEncoder _urlEncoder;
        private readonly IEventLogService _eventLogService;

        private const string AuthenticatorUriFormat = "otpauth://totp/{0}:{1}?secret={2}&issuer={0}&digits=6";

        public EnableAuthenticatorModel
        (
            UserManager<ApplicationUser> userManager,
            ILogger<EnableAuthenticatorModel> logger,
            UrlEncoder urlEncoder,
            IEventLogService eventLogSvc
        )
        {
            _userManager = userManager;
            _logger = logger;
            _urlEncoder = urlEncoder;
            _eventLogService = eventLogSvc;
        }

        public string SharedKey { get; set; }

        public string AuthenticatorUri { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required]
            [StringLength(7, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Text)]
            [Display(Name = "Verification Code")]
            public string Code { get; set; }
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            Console.WriteLine("[" + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + "][EnableAuthenticatorModel][OnGetAsync] => (user == null): " + (user == null));
            
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }
            if (user != null)
            {
                Console.WriteLine("[" + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + "][EnableAuthenticatorModel][OnGetAsync] => (user.UserName): " + (user.UserName));
            }

            await LoadSharedKeyAndQrCodeUriAsync(user);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            Console.WriteLine("[" + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + "][EnableAuthenticatorModel][OnPostAsync] => (user == null): " + (user == null));

            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }
            if (user != null)
            {
                Console.WriteLine("[" + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + "][EnableAuthenticatorModel][OnPostAsync] => (user.UserName): " + (user.UserName));
            }

            if (!ModelState.IsValid)
            {
                await LoadSharedKeyAndQrCodeUriAsync(user);
                return Page();
            }

            // Strip spaces and hypens
            var verificationCode = Input.Code.Replace(" ", string.Empty).Replace("-", string.Empty);
            Console.WriteLine("[" + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + "][EnableAuthenticatorModel][OnPostAsync][" 
                                    + user.UserName + "] => (verificationCode): '" + verificationCode + "'");

            var is2faTokenValid = await _userManager.VerifyTwoFactorTokenAsync(
                user, _userManager.Options.Tokens.AuthenticatorTokenProvider, verificationCode);

            Console.WriteLine("[" + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + "][EnableAuthenticatorModel][OnPostAsync]["
                                + user.UserName + "] => (is2faTokenValid): " + (is2faTokenValid));

            if (!is2faTokenValid)
            {
                ModelState.AddModelError("Input.Code", "Verification code is invalid.");
                await LoadSharedKeyAndQrCodeUriAsync(user);
                return Page();
            }

            await _userManager.SetTwoFactorEnabledAsync(user, true);
            _logger.LogInformation("User with ID '{UserId}' has enabled 2FA with an authenticator app.", user.Id);

            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            // BAD CODE:
            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            // Console.WriteLine("[" + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + "][EnableAuthenticatorModel][OnPostAsync]["
            //                         + user.UserName + "] => User with ID '{UserId}' has enabled 2FA with an authenticator app.", user.Id);
            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

            StringBuilder sb = new StringBuilder("[");
            sb.Append(DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"));
            sb.Append("]");
            sb.Append("[EnableAuthenticatorModel][OnPostAsync]");
            sb.Append("[");
            sb.Append(user.UserName);
            sb.Append("]");
            sb.Append(" => ");
            sb.Append("User with ID '");
            sb.Append(user.Id);
            sb.Append("' has enabled 2FA with an authenticator app.");
            Console.WriteLine(sb.ToString());

            ///////////////////////////////////////////////////////////////////
            // Log the 'TWO_FACTOR_ENABLED' event
            ///////////////////////////////////////////////////////////////////
            _eventLogService.LogEvent(EventTypeCodeConstants.TWO_FACTOR_ENABLED, user); 

            ///////////////////////////////////////////////////////////////////////////////////////////
            // IMPORTANT
            ///////////////////////////////////////////////////////////////////////////////////////////
            // Intentionally bypassing the 'ShowRecoveryCodes' page
            // REASON: Simplify User Interface based on user feedback (UAT)
            ///////////////////////////////////////////////////////////////////////////////////////////
            // var recoveryCodes = await _userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, 10);
            // TempData["RecoveryCodes"] = recoveryCodes.ToArray();
            // return RedirectToPage("./ShowRecoveryCodes");
            ///////////////////////////////////////////////////////////////////////////////////////////
            return RedirectToPage("../../Index");
        }

        private async Task LoadSharedKeyAndQrCodeUriAsync(ApplicationUser user)
        {
            // Load the authenticator key & QR code URI to display on the form
            var unformattedKey = await _userManager.GetAuthenticatorKeyAsync(user);
            if (string.IsNullOrEmpty(unformattedKey))
            {
                await _userManager.ResetAuthenticatorKeyAsync(user);
                unformattedKey = await _userManager.GetAuthenticatorKeyAsync(user);
            }

            SharedKey = FormatKey(unformattedKey);
            AuthenticatorUri = GenerateQrCodeUri(user.Email, unformattedKey);
        }

        private string FormatKey(string unformattedKey)
        {
            var result = new StringBuilder();
            int currentPosition = 0;
            while (currentPosition + 4 < unformattedKey.Length)
            {
                result.Append(unformattedKey.Substring(currentPosition, 4)).Append(" ");
                currentPosition += 4;
            }
            if (currentPosition < unformattedKey.Length)
            {
                result.Append(unformattedKey.Substring(currentPosition));
            }

            return result.ToString().ToLowerInvariant();
        }

        private string GenerateQrCodeUri(string email, string unformattedKey)
        {
            return string.Format(
                AuthenticatorUriFormat,
                _urlEncoder.Encode("GSA Learning Academy"),
                _urlEncoder.Encode(email),
                unformattedKey);
        }
    }
}
