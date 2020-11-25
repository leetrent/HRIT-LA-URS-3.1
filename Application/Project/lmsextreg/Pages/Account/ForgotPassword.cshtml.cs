using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using lmsextreg.Data;
using lmsextreg.Services;
using lmsextreg.Constants;
using lmsextreg.Utils;  

namespace lmsextreg.Pages.Account
{
    [AllowAnonymous]
    public class ForgotPasswordModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _emailSender;
        private readonly IConfiguration _configuration;
        private readonly ILogger<LoginModel> _logger;
        private readonly IReCaptchaService _reCaptchaService;

        public ForgotPasswordModel(UserManager<ApplicationUser> userManager, IEmailSender emailSender, 
                                    IConfiguration config, ILogger<LoginModel> logger, IReCaptchaService reCaptchaService)
        {
            _userManager = userManager;
            _emailSender = emailSender;
            _configuration = config;
            _logger = logger;
            _reCaptchaService = reCaptchaService;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }
        }

        public void OnGet()
        {
            Console.WriteLine("[ForgotPassword.OnGet]: BEGIN");
            // I'm not a robot
            ViewData["ReCaptchaKey"] = _configuration[MiscConstants.GOOGLE_RECAPTCHA_KEY]; 
            Console.WriteLine("[ForgotPassword.OnGet]: END");
        }

        public async Task<IActionResult> OnPostAsync()
        {
            Console.WriteLine("[ForgotPassword.OnPostAsync]: BEGIN");

            ///////////////////////////////////////////////////////////////////   
            // "I'm not a robot" check ...
            ///////////////////////////////////////////////////////////////////   
            if  ( ! _reCaptchaService.ReCaptchaPassed
                    (
                        Request.Form["g-recaptcha-response"],
                        _configuration[MiscConstants.GOOGLE_RECAPTCHA_SECRET]
                    )
                )
            {
                Console.WriteLine("[Login.OnPostAsync] reCAPTCHA FAILED");
                ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                // RECAPTCHA FAILED - redisplay form
                ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                ModelState.AddModelError(string.Empty, "You failed the CAPTCHA. Are you a robot?");
                ViewData["ReCaptchaKey"] = _configuration[MiscConstants.GOOGLE_RECAPTCHA_KEY];
                return Page();
            }
            Console.WriteLine("[Login.OnPostAsync] reCAPTCHA PASSED");

            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(Input.Email);
                if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return RedirectToPage("./ForgotPasswordConfirmation");
                }

                // For more information on how to enable account confirmation and password reset please 
                // visit https://go.microsoft.com/fwlink/?LinkID=532713
                var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                var callbackUrl = Url.ResetPasswordCallbackLink(user.Id, code, Request.Scheme);
                await _emailSender.SendResetPasswordAsync(Input.Email, callbackUrl);
                return RedirectToPage("./ForgotPasswordConfirmation");
            }

            return Page();
        }
    }
}
