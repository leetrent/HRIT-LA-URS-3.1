using System.Linq;
using System.Collections.Generic;
using lmsextreg.Models;
using lmsextreg.Constants;

namespace lmsextreg.Utils
{
    public static class EnrollmentStatusUtil
    {
        private static readonly string[] STATUS_CODES = new string[] 
        { 
            StatusCodeConstants.PENDING,
            StatusCodeConstants.APPROVED,
            StatusCodeConstants.DENIED,
            StatusCodeConstants.REVOKED,
            StatusCodeConstants.WITHDRAWN
        };
        public static IList<EnrollmentStatusCount> GroupByStatusCode(IList<ProgramEnrollment> orginalList, IQueryable<EnrollmentStatus> enrollmentStatusList)
        {
            List <EnrollmentStatusCount> groupByList = new List<EnrollmentStatusCount>();
            // foreach (var statusCode in STATUS_CODES)
            // {
            //     groupByList.Add(new EnrollmentStatusCount(statusCode, EnrollmentStatusUtil.CountByStatusCode(orginalList, statusCode)));
            // }
            foreach (var enrollmentStatus in enrollmentStatusList)
            {
                groupByList.Add
                (
                    new EnrollmentStatusCount
                    (
                        enrollmentStatus.StatusCode, 
                        enrollmentStatus.StatusLabel, 
                        EnrollmentStatusUtil.CountByStatusCode(orginalList, enrollmentStatus.StatusCode)
                    )
                );
            }
            return groupByList;

        }
        public static int CountByStatusCode(IList<ProgramEnrollment> orginalList, string statusCode)
        {
            return orginalList.Where(pe => pe.StatusCode == statusCode).ToList().Count();
        }
    }
}