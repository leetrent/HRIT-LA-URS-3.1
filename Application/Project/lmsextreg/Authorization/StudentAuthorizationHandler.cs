using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Identity;
using lmsextreg.Models;
using lmsextreg.Constants;
using lmsextreg.Data;

namespace lmsextreg.Authorization
{
    public class StudentAuthorizationHandler
        : AuthorizationHandler<OperationAuthorizationRequirement, ProgramEnrollment>
    {
        UserManager<ApplicationUser> _userManager;

        public StudentAuthorizationHandler(UserManager<ApplicationUser> userMgr)
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
                    Console.WriteLine("StudentAuthorizationHandler: FAILED (null check)");
                    return Task.CompletedTask;
                }

                if  (   authRequirement.Name != CRUDConstants.CREATE    &&
                        authRequirement.Name != CRUDConstants.RETRIEVE  &&
                        authRequirement.Name != CRUDConstants.UPDATE    &&
                        authRequirement.Name != CRUDConstants.DELETE
                    )
                {
                    Console.WriteLine("StudentAuthorizationHandler: FAILED (unauthorized operation)");
                    return Task.CompletedTask;
                }

                if ( authResource.StudentUserId == _userManager.GetUserId(authContext.User) )
                {
                    Console.WriteLine("StudentAuthorizationHandler: SUCCESS (authorization check PASSED)");
                    authContext.Succeed(authRequirement);
                }
                
                Console.WriteLine("StudentAuthorizationHandler: FAILED (unauthorized resource)");
                return Task.CompletedTask;
            }
    }
}