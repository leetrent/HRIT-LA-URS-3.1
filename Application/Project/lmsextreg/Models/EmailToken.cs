using lmsextreg.Data;

namespace lmsextreg.Models
{
    public class EmailToken
    {
        public string UserId { get; set; }

        public string TokenValue { get; set; }

        public ApplicationUser User { get; set; }
    }
}