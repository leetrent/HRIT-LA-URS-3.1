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
    public class CanAccessProfileLinkHandler : AuthorizationHandler<CanAccessProfileLink>
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ISessionCookieService _sessionCookieService;
        private readonly ILogger<CanAccessProfileLinkHandler> _logger;        

        public CanAccessProfileLinkHandler  (   SignInManager<ApplicationUser> signInMgr,
                                                ISessionCookieService sessionCookieSvc,
                                                ILogger<CanAccessProfileLinkHandler> logger
                                            )
        {
            _signInManager = signInMgr;
            _sessionCookieService = sessionCookieSvc;
            _logger = logger; 

        }        

        protected override Task HandleRequirementAsync (AuthorizationHandlerContext authContext,
                                                        CanAccessProfileLink requirement)
        {
            bool isSignedIn                 = false;
            bool isTwoFactorEnabled         = false;
            bool isStudentOrApproverOrAdmin = false;
            string appUserId                = null;
            string userName                 = "null";
            ApplicationUser appUser         = null;

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
                        bool isStudent              = authContext.User.IsInRole("STUDENT");
                        bool isApprover             = authContext.User.IsInRole("APPROVER");
                        bool isAdmin                = authContext.User.IsInRole(RoleConstants.ADMIN);
                        isStudentOrApproverOrAdmin  = (isStudent || isApprover || isAdmin);
                    }
                }
            }

            string logSnippet = new StringBuilder("[")
                    .Append(DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"))
                    .Append("][CanAccessProfileLinkHandler] => ")
                    .ToString();

            Console.WriteLine(logSnippet + $"({userName})(isSignedIn)................: {isSignedIn}");
            Console.WriteLine(logSnippet + $"({userName}(isTwoFactorEnabled).........: {isTwoFactorEnabled}");
            Console.WriteLine(logSnippet + $"({userName}(isStudentOrApproverOrAdmin).: {isStudentOrApproverOrAdmin}");

            if ( isSignedIn 
                    && isTwoFactorEnabled 
                    && isStudentOrApproverOrAdmin)
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