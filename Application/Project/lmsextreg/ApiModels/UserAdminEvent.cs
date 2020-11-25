using System.Text;
using lmsextreg.Data;
using lmsextreg.Constants;
using System;

namespace lmsextreg.ApiModels
{
    public class UserAdminEvent
    {
        public string EventTypeCode     { get; }
        public string AdminCreatedId    { get; }
        public string AdminCreatedEmail { get; }
        public string UserId            { get; }
        public string UserEmail         { get; }
        public string DataValues        { get;  }

        public UserAdminEvent(string eventTypeCode, ApplicationUser admin, ApplicationUser user)
        {
            this.EventTypeCode      = eventTypeCode;
            this.AdminCreatedId     = admin.Id;
            this.AdminCreatedEmail  = admin.Email;
            this.UserId             = user.Id;
            this.UserEmail          = user.Email;

            StringBuilder sb = new StringBuilder("User = { ");
            sb.Append("userEmail: ");
            sb.Append(user.Email);

            if (eventTypeCode.Equals(EventTypeCodeConstants.ADMIN_UNLOCKED_ACCOUNT))
            {
                sb.Append(", accountLocked: ");
                sb.Append(user.LockoutEnd != null || user.LockoutEnd > DateTime.Now);
            }
            else if (eventTypeCode.Equals(EventTypeCodeConstants.ADMIN_CONFIRMED_EMAIL))
            {
                sb.Append(", emailConfirmed: ");
                sb.Append(user.EmailConfirmed);
            }
            else if (eventTypeCode.Equals(EventTypeCodeConstants.ADMIN_DISABLED_TWO_FACTOR_AUTH))
            {
                sb.Append(", twoFactorEnabled: ");
                sb.Append(user.TwoFactorEnabled);
            }
            else
            {
                sb.Append(", [INVALID EVENT TYPE CODE] ");
            }

            sb.Append(", adminEmail: ");
            sb.Append(admin.Email);
            sb.Append(" } ");

            this.DataValues = sb.ToString();
        }
    }
}
