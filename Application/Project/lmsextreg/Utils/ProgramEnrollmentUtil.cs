using System;
using System.Linq;
using System.Collections.Generic;
using lmsextreg.Models;
using lmsextreg.Constants;

namespace lmsextreg.Utils
{
    public static class ProgramEnrollmentUtil
    {
        private static readonly string[] SORTED_STATUS_CODES = new string[] 
        { 
            StatusCodeConstants.PENDING,
            StatusCodeConstants.APPROVED,
            StatusCodeConstants.DENIED,
            StatusCodeConstants.REVOKED,
            StatusCodeConstants.WITHDRAWN,
            StatusCodeConstants.NONE
        };

        public static IList<ProgramEnrollment> SortByStatusCode(IList<ProgramEnrollment> orginalList)
        {
            Console.WriteLine("[ProgramEnrollmentUtil][SortByStatusCode] => (orginalList == null): " + (orginalList == null));
            if ( orginalList != null)
            {
                Console.WriteLine("[ProgramEnrollmentUtil][SortByStatusCode] => (orginalList.Count): " + (orginalList.Count));
            }

            List <ProgramEnrollment> sortedList = new List<ProgramEnrollment>();

            foreach (var statusCode in SORTED_STATUS_CODES)
            {
                sortedList.AddRange(FilterByStatusCode(orginalList, statusCode));
            }

            // debug(sortedList);

            return sortedList;
        }

        public static IList<ProgramEnrollment> FilterByStatusCode(IList<ProgramEnrollment> orginalList, string statusCode)
        {
            return orginalList.Where(pe => pe.StatusCode == statusCode).ToList();
        }

        private static void debug(List <ProgramEnrollment> sortedList)
        {
            Console.WriteLine("[ProgramEnrollmentUtil][debug] => (sortedList.Count): " + (sortedList.Count));
            foreach ( var pe in sortedList)
            {
                Console.WriteLine("[ProgramEnrollmentUtil][debug] => (pe.StatusCode): " + (pe.StatusCode));
            }
        }
    }
}