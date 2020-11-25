using lmsextreg.Models;

namespace lmsextreg.Repositories
{
    public interface ISessionCookieRepository
    {
        void Create (SessionCookie sessionCookie);
        SessionCookie Retrieve (string userName, string cookieName);
        void Update (SessionCookie sessionCookie);

        void Delete (SessionCookie sessionCookie);
   }
}