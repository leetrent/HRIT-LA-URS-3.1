using lmsextreg.Models;

namespace lmsextreg.Repositories
{
    public interface IEmailTokenRepository
    {
        void Create (string userId, string tokenValue);
        EmailToken Retrieve (string userId);
   }
}