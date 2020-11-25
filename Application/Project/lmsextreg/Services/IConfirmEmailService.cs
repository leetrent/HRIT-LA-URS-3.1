using lmsextreg.Data;

namespace lmsextreg.Services
{
    public interface IConfirmEmailService
    {
        bool IsConfirmed(ApplicationUser appUser, string tokenValue);      
    }
}