using System.Linq;
using lmsextreg.Data;
using lmsextreg.Models;

namespace lmsextreg.Repositories
{
    public class EmailTokenRepository : IEmailTokenRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public EmailTokenRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public void Create (string userId, string tokenValue)
        {
            _dbContext.EmailTokens.Add(new EmailToken {UserId = userId, TokenValue = tokenValue});
            _dbContext.SaveChanges();
        }
        public EmailToken Retrieve (string userId)
        {
            return _dbContext.EmailTokens.Where(et => et.UserId == userId).SingleOrDefault();
        }
   }
}