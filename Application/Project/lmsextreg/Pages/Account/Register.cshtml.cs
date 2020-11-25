using System;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using System.Net.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using lmsextreg.Data;
using lmsextreg.Services;
using lmsextreg.Models;
using lmsextreg.Constants;
using lmsextreg.Utils;
using lmsextreg.Repositories;

namespace lmsextreg.Pages.Account
{
    [AllowAnonymous]    
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<LoginModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly ApplicationDbContext _dbContext;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly IEventLogService _eventLogService;
        private readonly IReCaptchaService _reCaptchaService;
        private readonly IEmailTokenRepository _emailTokenRepo;


        public RegisterModel
            (
                UserManager<ApplicationUser> userManager,
                SignInManager<ApplicationUser> signInManager,
                ILogger<LoginModel> logger,
                IEmailSender emailSender,
                ApplicationDbContext dbContext,
                RoleManager<IdentityRole> roleManager,
                IConfiguration config,
                IEventLogService eventLogSvc,
                IReCaptchaService reCaptchaService,
                IEmailTokenRepository emailTokenRepo
            )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _dbContext = dbContext;
            _roleManager = roleManager;
            _configuration = config;
            _eventLogService = eventLogSvc;
            _reCaptchaService = reCaptchaService;
            _emailTokenRepo = emailTokenRepo;
        }

        [BindProperty]
        public InputModel Input { get; set; }
        public SelectList AgencySelectList { get; set; }
        public SelectList SubAgencySelectList { get; set; }     
        public string ReturnUrl { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 12)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }

            [Required]
            [RegularExpression(@"^[a-zA-Z''-'\s]{1,40}$", ErrorMessage = "Only upper case characters, lower case characters and the hyphen character are allowed.")]
            [Display(Name = "First Name")]
            public string FirstName { get; set; }

            [RegularExpression(@"^[a-zA-Z''-'\s]{1,40}$", ErrorMessage = "Only upper case characters, lower case characters and the hyphen character are allowed.")]
            [Display(Name = "Middle Name")]
            public string MiddleName { get; set; }

            [Required]
            [RegularExpression(@"^[a-zA-Z''-'\s]{1,40}$", ErrorMessage = "Only upper case characters, lower case characters and the hyphen character are allowed.")]
            [Display(Name = "Last Name")]
            public string LastName { get; set; }

            [Required]
            [RegularExpression(@"^[a-zA-Z''-'\s]{1,40}$", ErrorMessage = "Only upper case characters, lower case characters and the hyphen character are allowed.")]
            [Display(Name = "Job Title")]
            public string JobTitle { get; set; }   

            [Required]
            [Display(Name = "Agency")]  
            public string AgencyID { get; set; }

            [Required]
            [Display(Name = "SubAgency")]  
            public string SubAgencyID { get; set; }    

            [BindProperty]
            [Display(Name = "I agree to these Rules of Behavior")]
            [Range(typeof(bool), "true", "true", ErrorMessage = "Rules of Behavior must be agreed to in order to register.")]
            public bool RulesOfBehaviorAgreedTo { get; set; }            
        }

         public void OnGet(string returnUrl = null)
        {
            AgencySelectList    = new SelectList(_dbContext.Agencies.OrderBy(a => a.DisplayOrder), "AgencyID", "AgencyName");
            SubAgencySelectList = new SelectList(_dbContext.SubAgencies.OrderBy(sa => sa.DisplayOrder), "SubAgencyID", "SubAgencyName");
            
            var recaptchaKey = _configuration[MiscConstants.GOOGLE_RECAPTCHA_KEY];
            //Console.WriteLine("recaptchaKey: " + recaptchaKey);
            
            var recaptchaSecret = _configuration[MiscConstants.GOOGLE_RECAPTCHA_SECRET];
            //Console.WriteLine("recaptchaSecret: " + recaptchaSecret);              

            ViewData["ReCaptchaKey"] = _configuration[MiscConstants.GOOGLE_RECAPTCHA_KEY];            
         
            ReturnUrl = PageModelUtil.EnsureLocalUrl(this, returnUrl);
         }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
 			if ( ! ModelState.IsValid )
			{
                Console.WriteLine("[" + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + "][Register][OnPost] => Modelstate is INVALID - returning Page()");
				return Page();
			} 

            Console.WriteLine("[" + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + "][Register][OnPost][" + Input.Email + "] => Modelstate is VALID - continuing ...");            
 
            ReturnUrl = PageModelUtil.EnsureLocalUrl(this, returnUrl);

            ///////////////////////////////////////////////////////////////////   
            // "I'm not a robot" check ...
            ///////////////////////////////////////////////////////////////////   
            if  ( ! _reCaptchaService.ReCaptchaPassed
                    (
                        Request.Form["g-recaptcha-response"], // that's how you get it from the Request object
                        _configuration[MiscConstants.GOOGLE_RECAPTCHA_SECRET]
                    )
                )
            {
                Console.WriteLine("[" + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + "][Register][OnPost][" + Input.Email + "] => reCAPTCHA FAILED - returning Page()");
                ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                // RECAPTCHA FAILED - redisplay form
                ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                ModelState.AddModelError(string.Empty, "You failed the CAPTCHA. Are you a robot?");
                AgencySelectList    = new SelectList(_dbContext.Agencies.OrderBy(a => a.DisplayOrder), "AgencyID", "AgencyName");
                SubAgencySelectList = new SelectList(_dbContext.SubAgencies.OrderBy(sa => sa.DisplayOrder), "SubAgencyID", "SubAgencyName");
                ViewData["ReCaptchaKey"] = _configuration[MiscConstants.GOOGLE_RECAPTCHA_KEY];
                return Page();
            }

            Console.WriteLine("[" + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + "][Register][OnPost][" + Input.Email + "] => reCAPTCHA PASSED - continuing ...");  

            if (ModelState.IsValid)
            {
                Console.WriteLine("[" + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + "][Register][OnPost][" + Input.Email + "] => Creating ApplicationUser ...");

                var user = new ApplicationUser
                { 
                    UserName                = Input.Email, 
                    Email                   = Input.Email,
                    FirstName               = Input.FirstName,
                    MiddleName              = Input.MiddleName,
                    LastName                = Input.LastName,
                    JobTitle                = Input.JobTitle,
                    AgencyID                = Input.AgencyID,
                    SubAgencyID             = Input.SubAgencyID,
                    DateRegistered          = DateTime.Now,
                    DateAccountExpires      = DateTime.Now.AddDays(AccountConstants.DAYS_ACCOUNT_EXPIRES),
                    DatePasswordExpires     =  DateTime.Now.AddDays(AccountConstants.DAYS_PASSWORD_EXPIRES),
                    RulesOfBehaviorAgreedTo = Input.RulesOfBehaviorAgreedTo
                };

                ///////////////////////////////////////////////////////////////////
                // Create User
                ///////////////////////////////////////////////////////////////////
                Console.WriteLine("[" + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + "][Register][OnPost][" + user.Email + "] => Calling _userManager.CreateAsync() ...");
                var result = await _userManager.CreateAsync(user, Input.Password);

                ///////////////////////////////////////////////////////////////////
                // Create User Role 
                ///////////////////////////////////////////////////////////////////
                if (result.Succeeded)
                {
                    Console.WriteLine("[" + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + "][Register][OnPost][" + user.Email + "] => Calling _userManager.AddToRoleAsync() ...");
                    result = await _userManager.AddToRoleAsync(user, RoleConstants.STUDENT);
                }

                if (result.Succeeded)
                {
                    Console.WriteLine("[" + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + "][Register][OnPost][" + user.Email + "] => User created with role of STUDENT ...");

                    ////////////////////////////////////////////////////////////////////////
                    // OLD WAY OF GENERATING EMAIL VERIFICATION TOKENS
                    ////////////////////////////////////////////////////////////////////////
                    // var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    ////////////////////////////////////////////////////////////////////////

                    ////////////////////////////////////////////////////////////////////////
                    // NEW WAY OF GENERATING EMAIL VERIFICATION TOKENS
                    ////////////////////////////////////////////////////////////////////////
                    string emailToken = Guid.NewGuid().ToString();
                    _emailTokenRepo.Create(user.Id, emailToken);
                    ////////////////////////////////////////////////////////////////////////

                    Console.WriteLine("[" + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + "][Register][OnPost][" + user.Email + "] => BEGIN: Email Confirmation Token:");
                    Console.WriteLine("'" + emailToken + "'");
                    Console.WriteLine("[" + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + "][Register][OnPost][" + user.Email + "] => END: Email Confirmation Token");

                    Console.WriteLine("[" + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + "][Register][OnPost][" + user.Email + "] => HttpRequest.Scheme: '" + Request.Scheme + "'");

                    //var callbackUrl = Url.EmailConfirmationLink(user.Id, code, Request.Scheme);
                    var callbackUrl = Url.EmailConfirmationLink(user.Id, emailToken, "https");

                    Console.WriteLine("[" + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + "][Register][OnPost][" + user.Email + "] => BEGIN: EmailConfirmationLink:");
                    Console.WriteLine(callbackUrl);
                    Console.WriteLine("[" + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + "][Register][OnPost][" + user.Email + "] => END: EmailConfirmationLink");

                    Console.WriteLine("[" + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + "][Register][OnPost][" + user.Email + "] => Sedning email confirmation with link ...");
                    await _emailSender.SendEmailConfirmationAsync(Input.Email, callbackUrl);

                    //await _signInManager.SignInAsync(user, isPersistent: false);
                    //return LocalRedirect(Url.GetLocalUrl(returnUrl));

                    ///////////////////////////////////////////////////////////////////
                    // Log the 'USER_REGISTERED' event
                    ///////////////////////////////////////////////////////////////////
                   _eventLogService.LogEvent(EventTypeCodeConstants.USER_REGISTERED, user);

                    /////////////////////////////////////////////////////////////////////
                    // Redirect user to 'RegisterConfirmation' page
                    /////////////////////////////////////////////////////////////////////
                    return RedirectToPage("./RegisterConfirmation");
                }
                
                _logger.LogDebug("[logger] # of errors: " + result.Errors.Count());
                Console.WriteLine("[console] # of errors: " + result.Errors.Count());

                foreach (var error in result.Errors)
                {
                    _logger.LogDebug(error.Description);
                    Console.WriteLine(error.Description);

                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            // If we got this far, something failed, redisplay form
            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            AgencySelectList    = new SelectList(_dbContext.Agencies.OrderBy(a => a.DisplayOrder), "AgencyID", "AgencyName");
            SubAgencySelectList = new SelectList(_dbContext.SubAgencies.OrderBy(sa => sa.DisplayOrder), "SubAgencyID", "SubAgencyName");
            ViewData["ReCaptchaKey"] = _configuration[MiscConstants.GOOGLE_RECAPTCHA_KEY];
            return Page();
        }

       public JsonResult OnGetSubAgenciesInAgency(string agyID) 
        {
            List<SubAgency> subAgencyList = _dbContext.SubAgencies.Where( sa => sa.AgencyID == agyID ).OrderBy(sa => sa.DisplayOrder).ToList();
            return new JsonResult(new SelectList(subAgencyList, "SubAgencyID", "SubAgencyName"));
        } 
    }
}