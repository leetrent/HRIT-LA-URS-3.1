using System;

namespace lmsextreg.Models
{
    public class EnrollmentStatusCount
    {
        public EnrollmentStatusCount(string code, string label, int count)
        {
            this.StatusCode  = code;
            this.StatusLabel = label;
            this.StatusCount = count;
        }
        public string StatusCode { get; }
        public string StatusLabel { get; }
        public int StatusCount { get; }
    }
}