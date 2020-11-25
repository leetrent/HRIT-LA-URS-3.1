
using System;
using System.Linq;
using lmsextreg.Data;
using lmsextreg.Models;

namespace lmsextreg.Repositories
{
    public class SessionCookieRepository : ISessionCookieRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public SessionCookieRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void Create (SessionCookie sessionCookie)
        {           
            _dbContext.SessionCookies.Add(sessionCookie);
            _dbContext.SaveChanges();
        }
        
        public SessionCookie Retrieve (string userName, string cookieName)
        {
            return _dbContext.SessionCookies.Where(ac => ac.UserName == userName && ac.CookieName == cookieName)
                                            .SingleOrDefault();
        }
        
        public void Update (SessionCookie sessionCookie)
        {
            _dbContext.SessionCookies.Update(sessionCookie);
            _dbContext.SaveChanges();
        }
        public void Delete (SessionCookie sessionCookie)
        {
            _dbContext.SessionCookies.Remove(sessionCookie);
            _dbContext.SaveChanges();
        }
    }
}