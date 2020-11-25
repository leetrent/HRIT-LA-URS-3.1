using System.Linq;
using lmsextreg.Data;

namespace lmsextreg.Repositories
{
    public interface IUserRepository
    {
        ApplicationUser RetrieveUserByNormalizedEmail(string normalizedEmail);
        ApplicationUser RetrieveUserByUserId(string userId);
        IQueryable<ApplicationUser> RetrieveAllUsers();
        int UpdateUser(ApplicationUser appUser);
    }
}