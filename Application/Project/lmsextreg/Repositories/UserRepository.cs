using System.Linq;
using Microsoft.EntityFrameworkCore;
using lmsextreg.Data;

namespace lmsextreg.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public UserRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public ApplicationUser RetrieveUserByNormalizedEmail(string normalizedEmail)
        {
            return _dbContext.ApplicationUsers.Where(au => au.NormalizedEmail == normalizedEmail).SingleOrDefault();
        }

        public ApplicationUser RetrieveUserByUserId(string userId)
        {
            return _dbContext.ApplicationUsers.Where(au => au.Id == userId).Include(au => au.SubAgency).ThenInclude(sa => sa.Agency).SingleOrDefault();
        }
        public IQueryable<ApplicationUser> RetrieveAllUsers()
        {
            return _dbContext.ApplicationUsers.OrderBy(au => au.Email);
        }

        public int UpdateUser(ApplicationUser appUser)
        {
            _dbContext.ApplicationUsers.Update(appUser);
            return _dbContext.SaveChanges();
        }
    }
}