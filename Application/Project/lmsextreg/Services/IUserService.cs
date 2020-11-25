using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using lmsextreg.Data;

namespace lmsextreg.Services
{
    public interface IUserService
    {
        ApplicationUser RetrieveUserByEmailAddress(string emailAddress);
        ApplicationUser RetrieveUserByUserId(string userId);
        List<ApplicationUser> RetrieveAllUsers();
        int UnlockUser(string userId);
        int ConfirmEmailAddress(string userId);
        int DisableTwoFactorAuth(string userId);
    }
}
