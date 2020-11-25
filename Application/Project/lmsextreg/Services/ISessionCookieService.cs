
using Microsoft.AspNetCore.Http;
using lmsextreg.Models;

namespace lmsextreg.Services
{
    public interface ISessionCookieService
    {
       void RecordSessionCookie(string userName, IRequestCookieCollection cookies);
       SessionCookie RetrieveSessionCookie(string userName);
       void RefreshSessionCookie(string userName);
       void RemoveSessionCookie(string userName);
       string DeterminePageRedirect(string userName, string tagName);
       bool UserHasAuthenticated(string userName);
    }
}