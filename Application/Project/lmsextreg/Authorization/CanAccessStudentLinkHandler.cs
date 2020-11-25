using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using lmsextreg.Data;
using lmsextreg.Constants;
using lmsextreg.Services;

namespace lmsextreg.Authorization
{
    public class CanAccessStudentLinkHandler : AuthorizationHandler<CanAccessStudentLink>
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ISessionCookieService _sessionCookieService;
        private readonly ILogger<CanAccessStudentLinkHandler> _logger;        

        public CanAccessStudentLinkHandler( SignInManager<ApplicationUser> signInMgr,
                                            ISessionCookieService sessionCookieSvc,
                                            ILogger<CanAccessStudentLinkHandler> logger )
        {
            _signInManager = signInMgr;
            _sessionCookieService = sessionCookieSvc;
            _logger = logger;            
        }        

        protected override Task HandleRequirementAsync (AuthorizationHandlerContext authContext,
                                                        CanAccessStudentLink requirement)
        {
            bool isSignedIn         = false;
            bool isTwoFactorEnabled = false;
            bool isStudent          = false;
            string userName         = null;

            string appUserId = _signInManager.UserManager.GetUserId(authContext.User);
            ApplicationUser appUser = GetUserAsync(appUserId).Result;

            if ( appUser == null ) 
            {
                isSignedIn  = false;
                userName    = "null";
            }
            else
            {
                isSignedIn = _signInManager.IsSignedIn(authContext.User)
                                && _sessionCookieService.UserHasAuthenticated(appUser.UserName);
                userName = appUser.UserName;              
            }

            if ( isSignedIn )
            {
                if ( String.IsNullOrEmpty(appUserId) == false )
                {
                    if ( appUser != null) 
                    {
                        isTwoFactorEnabled = appUser.TwoFactorEnabled;
                    }

                    if ( authContext != null && authContext.User != null)
                    {
                        isStudent = authContext.User.IsInRole("STUDENT");
                    }
                }
            }
       
            // _logger.LogInformation("[CanAccessStudentLinkHandler] - (" + userName + ")(isSignedInn)........: " + isSignedIn);
            // _logger.LogInformation("[CanAccessStudentLinkHandler] - (" + userName + ")(isTwoFactorEnabled).: " + isTwoFactorEnabled);
            // _logger.LogInformation("[CanAccessStudentLinkHandler] - (" + userName + ")(isStudent)..........: " + isStudent);

            if ( isSignedIn 
                    && isTwoFactorEnabled 
                    && isStudent )
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