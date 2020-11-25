using Microsoft.AspNetCore.Authorization.Infrastructure;
using lmsextreg.Constants;

namespace lmsextreg.Authorization
{
    public class AuthorizationOperations
    {
        public static OperationAuthorizationRequirement CREATE = 
            new OperationAuthorizationRequirement { Name = CRUDConstants.CREATE };
        public static OperationAuthorizationRequirement RETRIEVE = 
            new OperationAuthorizationRequirement { Name = CRUDConstants.RETRIEVE };   

        public static OperationAuthorizationRequirement UPDATE = 
            new OperationAuthorizationRequirement { Name = CRUDConstants.UPDATE };    

        public static OperationAuthorizationRequirement DELETE = 
            new OperationAuthorizationRequirement { Name = CRUDConstants.DELETE };       

        public static OperationAuthorizationRequirement APPROVE = 
            new OperationAuthorizationRequirement { Name = ApproverConstants.APPROVE };   

        public static OperationAuthorizationRequirement DENY = 
            new OperationAuthorizationRequirement { Name = ApproverConstants.DENY };               
    }
}