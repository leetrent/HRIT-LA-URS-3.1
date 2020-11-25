using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using lmsextreg.Data;
using lmsextreg.Constants;
using lmsextreg.Services;

namespace lmsextreg.Authorization
{
    public class CanAccessAdminLinkHandler : AuthorizationHandler<CanAccessAdminLink>
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ISessionCookieService _sessionCookieService;
        private readonly ILogger<CanAccessAdminLinkHandler> _logger;

        public CanAccessAdminLinkHandler(   SignInManager<ApplicationUser> signInMgr,
                                                ISessionCookieService sessionCookieSvc,
                                                ILogger<CanAccessAdminLinkHandler> logger
                                            )
        {
            _signInManager = signInMgr;
            _sessionCookieService = sessionCookieSvc;
            _logger = logger; 
        }        

        protected override Task HandleRequirementAsync (AuthorizationHandlerContext authContext,
                                                        CanAccessAdminLink requirement)
        {
            bool isSignedIn         = false;
            bool isTwoFactorEnabled = false;
            bool isAdmin            = false;
            string userName         = "null";
            ApplicationUser appUser = null;
            string appUserId        = null;

            if ( _signInManager != null && _sessionCookieService != null )
            {
                appUserId = _signInManager.UserManager.GetUserId(authContext.User);
                
                if ( String.IsNullOrEmpty(appUserId) == false )
                {
                    appUser = GetUserAsync(appUserId).Result;
                }

                if ( appUser != null )
                {
                    userName = appUser.UserName;
                    isSignedIn = _signInManager.IsSignedIn(authContext.User)
                                    && _sessionCookieService.UserHasAuthenticated(appUser.UserName);
                }
            }

            if ( isSignedIn )
            {
                if ( String.IsNullOrEmpty(appUserId) == false )
                {
                    if ( appUser != null) 
                    {
                        isTwoFactorEnabled = appUser.TwoFactorEnabled;
                        userName = appUser.UserName;
                    }

                    if ( authContext != null && authContext.User != null)
                    {
                        isAdmin = authContext.User.IsInRole(RoleConstants.ADMIN);
                    }
                }
            }

            string logSnippet = new StringBuilder("[")
                    .Append(DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"))
                    .Append("][CanAccessAdminLinkHandler] => ")
                    .ToString();

            Console.WriteLine(logSnippet + $"({userName})(isSignedIn)........: {isSignedIn}");
            Console.WriteLine(logSnippet + $"({userName}(isTwoFactorEnabled).: {isTwoFactorEnabled}");
            Console.WriteLine(logSnippet + $"({userName}(isAdmin)............: {isAdmin}");

            if ( isSignedIn 
                    && isTwoFactorEnabled 
                    && isAdmin )
            {
                authContext.Succeed(requirement);
            }            

            return Task.CompletedTask;
        }

        private  Task<ApplicationUser> GetUserAsync(string userId)
        {
            return _signInManager.UserManager.FindByIdAsync(userId);
        }
    }
}