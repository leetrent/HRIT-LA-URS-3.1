using System;
using System.Text;
using System.Collections;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using lmsextreg.Services;
using lmsextreg.Data;
using lmsextreg.ViewModels;
using System.Collections.Generic;

namespace lmsextreg.Pages.Admin
{
    [Authorize(Roles = "ADMIN")]
    public class UpdateModel : PageModel
    {
        private readonly IUserService _userService;
        [BindProperty]
        public ApplicationUser AppUser { get ; set; }

        public IList<AvailableActionCardViewModel> AvailableActionCardViewModels;


        public UpdateModel(IUserService userSvc)
        {
            string logSnippet = new StringBuilder("[")
                    .Append(DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"))
                    .Append("][Admin][Update][Constructor] => ")
                    .ToString();
            
            Console.WriteLine(logSnippet + $"(userSvc == null).....: '{userSvc == null}'");
            _userService = userSvc;            
            Console.WriteLine(logSnippet + $"(_userService == null): '{_userService == null}'");           
        }

         public void OnGet(string userId = null)
        {
            string logSnippet = new StringBuilder("[")
                    .Append(DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"))
                    .Append("][Admin][Update][HttpGet] => ")
                    .ToString();

            Console.WriteLine(logSnippet + $"(userId)..............: '{userId}'");
           

            if ( String.IsNullOrEmpty(userId) || String.IsNullOrWhiteSpace(userId) )
            {
                // TODO: Redirect to error page
            }
            
            Console.WriteLine(logSnippet + $"(_userService == null): '{_userService == null}'");
            Console.WriteLine(logSnippet + $"Calling _userService.RetrieveUserByUserId()");

            this.AppUser = _userService.RetrieveUserByUserId(userId);

            this.AvailableActionCardViewModels = new List<AvailableActionCardViewModel>();
            if (this.AppUser.LockoutEnd != null && this.AppUser.LockoutEnd > DateTime.Now)
            {
                string title        = "Unlock Account";
                string text         = "User will be able to login once their account has been unlocked";
                string buttonLabel  = "Unlock";
                string onSubmit     = $"javascript:unlockUserAccount('{this.AppUser.Id}');";
                string cardId       = "unlockAccountCard";
                string buttonId     = "unlockAccountButton";
                AvailableActionCardViewModel cardVM = new AvailableActionCardViewModel(title, text, buttonLabel, onSubmit, cardId, buttonId);
                this.AvailableActionCardViewModels.Add(cardVM);
            }
            if (this.AppUser.EmailConfirmed == false)
            {
                string title        = "Confirm Email Address";
                string text         = "User will be able to move forward with the registration process once their email address has been confirmed.";
                string buttonLabel  = "Confirm";
                string onSubmit     = $"javascript:confirmEmailAddress('{this.AppUser.Id}');";
                string cardId       = "confirmEmailCard";
                string buttonId     = "confirmEmailButton";
                AvailableActionCardViewModel cardVM = new AvailableActionCardViewModel(title, text, buttonLabel, onSubmit, cardId, buttonId);
                this.AvailableActionCardViewModels.Add(cardVM);
            }
            if (this.AppUser.TwoFactorEnabled == true)
            {
                string title        = "Disable 2-Factor Authentication";
                string text         = "This will allow user to re-establish 2-factor authentication.";
                string buttonLabel  = "Disable";
                string onSubmit     = $"javascript:disableTwoFactorAuth('{this.AppUser.Id}');";
                string cardId       = "disableTwoFactorCard";
                string buttonId     = "disableTwoFactorButton";
                AvailableActionCardViewModel cardVM = new AvailableActionCardViewModel(title, text, buttonLabel, onSubmit, cardId, buttonId);
                this.AvailableActionCardViewModels.Add(cardVM);
            }

            Console.WriteLine(logSnippet + $"Returning from _userService.RetrieveUserByUserId()");
            Console.WriteLine(logSnippet + $"(this.AppUser == null): '{ this.AppUser== null}'");
        }

        public JsonResult OnPostUnlockUserAccount(string userId)
        {
            string logSnippet = new StringBuilder("[")
                    .Append(DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"))
                    .Append("][Admin][UpdateModel][OnPostUnlockUserAccount] => ")
                    .ToString();

            Console.WriteLine(logSnippet + $"(userId: '{userId}'");
            int rowsUpdated = _userService.UnlockUser(userId);
            Console.WriteLine(logSnippet + $"(rowsUpdated: '{rowsUpdated}'");
            return new JsonResult(rowsUpdated);
        }

        public JsonResult onGetIsUserAccountLocked(string userId)
        {
            string logSnippet = new StringBuilder("[")
                    .Append(DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"))
                    .Append("][Admin][UpdateModel][onGetIsUserAccountLocked] => ")
                    .ToString();

            Console.WriteLine(logSnippet + $"(userId): '{userId}'");
            ApplicationUser appUser = _userService.RetrieveUserByUserId(userId);
            Console.WriteLine(logSnippet + $"(appUser == null)...: '{appUser == null}'");
            Console.WriteLine(logSnippet + $"(appUser.LockoutEnd): '{appUser.LockoutEnd}'");
            return new JsonResult(appUser.LockoutEnd != null);
        }
    }
}