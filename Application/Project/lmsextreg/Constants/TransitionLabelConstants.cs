namespace lmsextreg.Constants
{
    public class TransitionLabelConstants
    {
        public static readonly string NONE_TO_PENDING       = "Initial enrollment request";
        public static readonly string PENDING_TO_WITHDRAWN  = "Applicant withdrew pending enrollment request";
        public static readonly string PENDING_TO_APPROVED   = "Enrollment request has been approved";
        public static readonly string PENDING_TO_DENIED     = "Enrollment request has been denied";  
        public static readonly string APPROVED_TO_WITHDRAWN = "Applicant withdrew approved enrollment request";
        public static readonly string APPROVED_TO_REVOKED   = "Approved enrollment has been revoked";
        public static readonly string WITHDRAWN_TO_PENDING  = "Applicant reenrollment request after enrollment request withdrawl";
        public static readonly string DENIED_TO_APPROVED    = "Previously denied enrollment request has now been appproved";
        public static readonly string DENIED_TO_PENDING    = "Applicant reenrollment request after enrollment had been denied";
        public static readonly string REVOKED_TO_PENDING    = "Applicant reenrollment request after enrollment had been revoked";
    }
}