using System;
using System.Text;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Diagnostics;
using lmsextreg.Data;
using lmsextreg.Services;

namespace lmsextreg.Pages
{
    [AllowAnonymous]
    public class ErrorModel : PageModel
    {
        public string RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

        private readonly UserManager<ApplicationUser> _userManager;

        private readonly IEmailSender _emailSender;

        public ErrorModel(UserManager<ApplicationUser> usrMgr, IEmailSender emailSender)
        {
            _userManager = usrMgr;
            _emailSender = emailSender;
        }

        //public void OnGet()
        public async Task OnGetAsync()
        {
            string logSnippet = new StringBuilder("[")
                                .Append(DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"))
                                .Append("][ErrorModel][OnGet] => ")
                                .ToString();

            Console.WriteLine(logSnippet + $"(_userManager == null).....: '{_userManager == null}'");
            Console.WriteLine(logSnippet + $"(HttpContext == null)......: '{HttpContext == null}'");

            if ( HttpContext != null)
            {
                Console.WriteLine(logSnippet + $"(HttpContext.User == null).: '{HttpContext.User == null}'");
            }          

             ApplicationUser appUser = null;
            if ( _userManager != null && HttpContext != null && HttpContext.User != null)
            {
                appUser = _userManager.GetUserAsync(HttpContext.User).Result;
                Console.WriteLine(logSnippet + $"(appUser == null): '{appUser == null}'");
                if ( appUser != null)
                {
                    Console.WriteLine(logSnippet + $"(appUser.Email): '{appUser.Email}'");
                }
            }

            var exceptionHandlerPathFeature = HttpContext.Features.Get<IExceptionHandlerFeature>();
            Console.WriteLine(logSnippet + $"(Activity.Current?.Id)..............: '{Activity.Current?.Id}'");
            Console.WriteLine(logSnippet + $"(HttpContext.TraceIdentifier).......: '{HttpContext.TraceIdentifier}'");
            Console.WriteLine(logSnippet + $"(exceptionHandlerPathFeature).......: '{exceptionHandlerPathFeature}'");
            Console.WriteLine(logSnippet + $"(exceptionHandlerPathFeature?.Error): '{exceptionHandlerPathFeature?.Error}'");   

            Exception ursException = exceptionHandlerPathFeature?.Error;      

            //RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;

            StringBuilder subject = new StringBuilder("URS Exception Encountered");
            string userEmailAddress = null;
            if ( appUser != null)
            {
                userEmailAddress = appUser.Email;
            }

            if (String.IsNullOrEmpty(userEmailAddress) == false 
                    && String.IsNullOrWhiteSpace(userEmailAddress) == false)
            {
                subject.Append($" - {userEmailAddress}");
            }
            else
            {
                subject.Append("- User Email Address IS NULL");
            }
            
            string message  = (ursException == null)
                            ? ("Exception IS NULL")
                            : (ursException.ToString());

            await _emailSender.SendEmailAsync("lee.trent@gsa.gov", subject.ToString(), message);
        }
    }
}
