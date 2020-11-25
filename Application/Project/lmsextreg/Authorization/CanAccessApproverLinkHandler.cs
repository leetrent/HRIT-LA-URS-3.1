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
    public class CanAccessApproverLinkHandler : AuthorizationHandler<CanAccessApproverLink>
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ISessionCookieService _sessionCookieService;
        private readonly ILogger<CanAccessApproverLinkHandler> _logger;

        public CanAccessApproverLinkHandler (   SignInManager<ApplicationUser> signInMgr,
                                                ISessionCookieService sessionCookieSvc,
                                                ILogger<CanAccessApproverLinkHandler> logger
                                            )
        {
            _signInManager = signInMgr;
            _sessionCookieService = sessionCookieSvc;
            _logger = logger; 
        }        

        protected override Task HandleRequirementAsync (AuthorizationHandlerContext authContext,
                                                        CanAccessApproverLink requirement)
        {
            bool isSignedIn         = false;
            bool isTwoFactorEnabled = false;
            bool isApprover         = false;
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
                        isApprover = authContext.User.IsInRole("APPROVER");
                    }
                }
            }
       
            // _logger.LogInformation("[CanAccesApproverLinkHandler] - (" + userName + ")(isSignedInn)........: " + isSignedIn);
            // _logger.LogInformation("[CanAccesApproverLinkHandler] - (" + userName + ")(isTwoFactorEnabled).: " + isTwoFactorEnabled);
            // _logger.LogInformation("[CanAccesApproverLinkHandler] - (" + userName + ")(isApprover).........: " + isApprover);

            if ( isSignedIn 
                    && isTwoFactorEnabled 
                    && isApprover )
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