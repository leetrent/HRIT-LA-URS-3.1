using System;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using lmsextreg.Repositories;
using lmsextreg.Models;
using lmsextreg.Constants;

namespace lmsextreg.Services
{
    public class SessionCookieService : ISessionCookieService
    {
        private readonly ISessionCookieRepository _sessionCookieRepo;
        private readonly ILogger<SessionCookieService> _logger;

        public SessionCookieService (   ISessionCookieRepository sessionCookieRepository,
                                        ILogger<SessionCookieService> logger
                                    )
        {
            _sessionCookieRepo = sessionCookieRepository;
            _logger = logger;
        }

        public void RecordSessionCookie(string userName, IRequestCookieCollection cookies)
        {
            _logger.LogDebug("[SessionCookieService][RecordSessionCookie] =>");
            
            if ( _logger.IsEnabled(LogLevel.Debug) )
            {
                foreach (var cookieKey in cookies.Keys)
                {
                    _logger.LogDebug("[SessionCookieService][RecordSessionCookie] => cookieKey: '" + cookieKey + "'");
                }
            }

            SessionCookie oldCookie = _sessionCookieRepo.Retrieve(userName, MiscConstants.SESSION_COOKIE_NAME);
            if ( oldCookie != null)
            {
                _logger.LogDebug("[SessionCookieService][RecordSessionCookie] => Deleting oldCookie");
                _sessionCookieRepo.Delete(oldCookie);
            }
            
            if ( cookies[MiscConstants.SESSION_COOKIE_NAME] == null ) 
            {
                StringBuilder sb = new StringBuilder("[SessionCookieService][RecordSessionCookie] => Session cookie '");
                sb.Append(MiscConstants.SESSION_COOKIE_NAME);
                sb.Append("' for User '");
                sb.Append(userName);
                sb.Append("' NOT FOUND!");
                string errMsg = sb.ToString();

                _logger.LogError(errMsg);
                throw new ApplicationException(errMsg);
            }

            SessionCookie newCookie = new SessionCookie
            {
                UserName = userName,
                CookieName = MiscConstants.SESSION_COOKIE_NAME,
                CookieValue = cookies[MiscConstants.SESSION_COOKIE_NAME],
                LastAccessedOn = DateTime.Now
            };
            
            if ( _logger.IsEnabled(LogLevel.Debug) )
            {
                _logger.LogDebug("[SessionCookieService][RecordSessionCookie]=> Saving new SessionCookie to the database: ");
                _logger.LogDebug(newCookie.ToString());
            }

            _sessionCookieRepo.Create(newCookie);
            
            _logger.LogDebug("<= [SessionCookieService][RecordSessionCookie]");            
        }

        public SessionCookie RetrieveSessionCookie(string userName)
        {
            if ( _logger.IsEnabled(LogLevel.Debug) )  
            {
                _logger.LogDebug("[SessionCookieService][RetrieveSessionCookie] => Retrieving SessionCookie for '" + userName +"'");
            }          

            return _sessionCookieRepo.Retrieve(userName, MiscConstants.SESSION_COOKIE_NAME);
        }

        public void RefreshSessionCookie(string userName)
        {
            if ( _logger.IsEnabled(LogLevel.Debug) )  
            {
                _logger.LogDebug("[SessionCookieService][RefreshSessionCookie] => Refreshing SessionCookie for '" + userName +"'");
            }     

            SessionCookie sessionCookie = this.RetrieveSessionCookie(userName);
            if ( sessionCookie == null )
            {
                StringBuilder sb = new StringBuilder("[SessionCookieService][RefreshSessionCookie] => SessionCookie for '");
                sb.Append(userName);
                sb.Append("' NOT FOUND! Throwing ApplicationException!");
                var errMsg = sb.ToString();

                _logger.LogError(errMsg);
                throw new ApplicationException(errMsg);
            }

            sessionCookie.LastAccessedOn = DateTime.Now;            
            _sessionCookieRepo.Update(sessionCookie);
        }        

        public void RemoveSessionCookie(string userName)
        {
            if ( _logger.IsEnabled(LogLevel.Debug) )  
            {
                _logger.LogDebug("[SessionCookieService][RemoveSessionCookie] => Removing SessionCookie for '" + userName +"'");
            }      

            SessionCookie sessionCookie = this.RetrieveSessionCookie(userName);
            if ( sessionCookie == null )
            {
                StringBuilder sb = new StringBuilder("[SessionCookieService][RemoveSessionCookie] => SessionCookie for '");
                sb.Append(userName);
                //sb.Append("' NOT FOUND! Throwing ApplicationException!");
                sb.Append("' NOT FOUND!");
                var errMsg = sb.ToString();

                // _logger.LogError(errMsg);
                _logger.LogInformation(errMsg);
                // throw new ApplicationException(errMsg);
            }   
            else
            {
                _sessionCookieRepo.Delete(sessionCookie); 
            }                           
        }

        public string DeterminePageRedirect(string userName, string tagName)
        {
           SessionCookie sessionCookie = this.RetrieveSessionCookie(userName);

            if ( sessionCookie == null )
            { 
                StringBuilder sb = new StringBuilder(tagName);
                sb.Append(" => SessionCookie for '");
                sb.Append(userName);
                sb.Append("' NOT FOUND. Redirecting to LoginPrep page.");
                _logger.LogInformation(sb.ToString());

                return "/Account/LoginPrep";   
            }

            if ( DateTime.Now > sessionCookie.LastAccessedOn.AddMinutes(30) )
            {
                StringBuilder sb = new StringBuilder(tagName);
                sb.Append(" => SessionCookie for '");
                sb.Append(userName);
                sb.Append("' HAS EXPIRED. It was last accessed on ");
                sb.Append(sessionCookie.LastAccessedOn);        

                if ( DateTime.Now > sessionCookie.LastAccessedOn.AddHours(2) )
                {
                    sb.Append(". Redirecting to LoginPrep page.");
                    _logger.LogInformation(sb.ToString());

                    return "/Account/LoginPrep";
                }
                else
                {
                    sb.Append(". Redirecting to SessionTimeout page.");
                    _logger.LogInformation(sb.ToString());                    

                    return "/Account/SessionTimeout";
                }
            }

            return null;
        }

        public bool UserHasAuthenticated(string userName)
        {
            SessionCookie sessionCookie = this.RetrieveSessionCookie(userName);

            if ( sessionCookie == null )
            {
                return false;
            }
            if ( DateTime.Now > sessionCookie.LastAccessedOn.AddMinutes(30) )
            {
                return false;
            }

            return true;
        }
    }
}