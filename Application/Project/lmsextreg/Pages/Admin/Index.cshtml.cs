using System;
using System.Text;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using lmsextreg.Services;
using lmsextreg.Data;


namespace lmsextreg.Pages.Admin
{
    [Authorize(Roles = "ADMIN")]
    public class IndexModel : PageModel
    {
        private readonly IUserService _userService;
        public List<ApplicationUser> Users { get; set; }
    

        public IndexModel(IUserService userSvc)
        {
            _userService = userSvc;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [EmailAddress]
            [Display(Name = "Email Address")]
            public string EmailAddress { get; set; }
        }

        public void OnGet(string emailAddress = null)
        {
            string logSnippet = new StringBuilder("[")
                    .Append(DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"))
                    .Append("][Admin][Index][HttpGet] => ")
                    .ToString();

            Console.WriteLine(logSnippet + $"(emailAddress): '{emailAddress}'");

            if ( String.IsNullOrEmpty(emailAddress) || String.IsNullOrWhiteSpace(emailAddress) )
            {
                this.Users = _userService.RetrieveAllUsers();
            }
            else
            {
                ApplicationUser appUser = _userService.RetrieveUserByEmailAddress(emailAddress);
                Console.WriteLine(logSnippet + $"(appUser == null): '{appUser == null}'");
                this.Users = new List<ApplicationUser>();
                if (appUser != null)
                {
                    this.Users.Add(appUser);
                }
            }
        }

        public IActionResult OnPostAsync(string returnUrl = null)
        {
            string logSnippet = new StringBuilder("[")
                    .Append(DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"))
                    .Append("][Admin][Index][OnPostAsync] => ")
                    .ToString();

            System.Console.WriteLine(logSnippet + $"(Input.EmailAddress): '{Input.EmailAddress}'");

            return RedirectToPage("Index", new { @emailAddress = Input.EmailAddress });
        }
    }
}