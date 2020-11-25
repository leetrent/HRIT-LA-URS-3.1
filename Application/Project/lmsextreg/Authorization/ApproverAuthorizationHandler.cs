using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Identity;
using lmsextreg.Models;
using lmsextreg.Constants;
using lmsextreg.Data;

namespace lmsextreg.Authorization
{
    public class ApproverAuthorizationHandler
        : AuthorizationHandler<OperationAuthorizationRequirement, ProgramEnrollment>
    {
        UserManager<ApplicationUser> _userManager;

        public ApproverAuthorizationHandler(UserManager<ApplicationUser> userMgr)
        {
            _userManager = userMgr;
        }
        
        protected override Task
            HandleRequirementAsync( AuthorizationHandlerContext authContext,
                                    OperationAuthorizationRequirement authRequirement,
                                    ProgramEnrollment authResource)
            {
                if (    authContext         == null || 
                        authContext.User    == null || 
                        authResource        == null 
                    )
                {
                    return Task.CompletedTask;
                }

                if ( ! authContext.User.IsInRole(RoleConstants.APPROVER))
                {
                    return Task.CompletedTask;
                }

                if  (   authRequirement.Name != ApproverConstants.APPROVE  &&
                        authRequirement.Name != ApproverConstants.DENY
                    )
                {
                    return Task.CompletedTask;
                }

                if ( authResource.ApproverUserId == _userManager.GetUserId(authContext.User) )
                {
                    authContext.Succeed(authRequirement);
                }

                return Task.CompletedTask;
            }
    }
}