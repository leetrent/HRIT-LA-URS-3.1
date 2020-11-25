using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc.Filters;
using lmsextreg.Data;
using lmsextreg.Models;
using lmsextreg.Constants;
using lmsextreg.Services;

namespace lmsextreg.Pages.Enrollments
{
    [Authorize(Roles = "STUDENT")]
    public class IndexModel : PageModel
    {
        private readonly lmsextreg.Data.ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<IndexModel> _logger;
        private readonly ISessionCookieService _sessionCookieService;

        public IndexModel ( lmsextreg.Data.ApplicationDbContext context,
                            UserManager<ApplicationUser> usrMgr,
                            ILogger<IndexModel> logger,
                            ISessionCookieService sessionCookieSvc)
        {
            _context = context;
            _userManager = usrMgr;
            _logger = logger;
            _sessionCookieService = sessionCookieSvc;
        }

        public string PENDING   = StatusCodeConstants.PENDING;
        public string APPROVED  = StatusCodeConstants.APPROVED;
        public string WITHDRAWN = StatusCodeConstants.WITHDRAWN;
        public string REVOKED   = StatusCodeConstants.REVOKED;
        public string DENIED    = StatusCodeConstants.DENIED;
        public IList<ProgramEnrollment> ProgramEnrollment { get;set; }

        public ApplicationUser LoggedInUser {get;set;}
        public bool ProgramsAreAvailable {get; set; }

        public async Task OnGetAsync()
        {
            LoggedInUser = await GetCurrentUserAsync();

            ProgramEnrollment = await _context.ProgramEnrollments
                .Where(p => p.StudentUserId == LoggedInUser.Id)
                .Include(p => p.LMSProgram)
                .Include ( pe => pe.Student).ThenInclude(s => s.Agency)
                .Include(p => p.EnrollmentStatus)
                .ToListAsync();
           
            var userID = _userManager.GetUserId(User);

            //////////////////////////////////////////////////////////////////////////
            // Select the remaining programs that student has net as yet enrolled in
            // This is used to manage the user interface to make sure that student
            // can't enroll in the same program more than once.
            /////////////////////////////////////////////////////////////////////////
            // PostgreSQL
            /////////////////////////////////////////////////////////////////////////
            // var sql = " SELECT * "
            //         + " FROM " + MiscConstants.DB_SCHEMA_NAME + ".\"LMSProgram\" "
            //         + " WHERE \"LMSProgramID\" "
            //         + " NOT IN "
            //         + " ( SELECT \"LMSProgramID\" "
            //         + "   FROM " + MiscConstants.DB_SCHEMA_NAME + ".\"ProgramEnrollment\" " 
            //         + "   WHERE \"StudentUserId\" = {0} "
            //         + " )";
            /////////////////////////////////////////////////////////////////////////
            // MySQL
            /////////////////////////////////////////////////////////////////////////
            var sql = " SELECT * "
                    + " FROM " + MiscConstants.DB_SCHEMA_NAME + ".LMSProgram "
                    + " WHERE LMSProgramID "
                    + " NOT IN "
                    + " ( SELECT LMSProgramID "
                    + "   FROM " + MiscConstants.DB_SCHEMA_NAME + ".ProgramEnrollment " 
                    + "   WHERE StudentUserId = {0} "
                    + " )";
             /////////////////////////////////////////////////////////////////////////        

            var resultSet =  _context.LMSPrograms.FromSqlRaw(sql, userID).AsNoTracking();
            ProgramsAreAvailable = (resultSet.Count() > 0);
        }

        private Task<ApplicationUser> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);

         public override void OnPageHandlerSelected(PageHandlerSelectedContext filterContext)
         {
            ApplicationUser appUser = _userManager.GetUserAsync(HttpContext.User).Result;
            SessionCookie sessionCookie = _sessionCookieService.RetrieveSessionCookie(appUser.UserName);

            if ( sessionCookie == null )
            {
                StringBuilder sb = new StringBuilder("[Enrollments][IndexModel][OnPageHandlerSelected] => SessionCookie for '");
                sb.Append(appUser.UserName);
                sb.Append("' NOT FOUND. Redirecting to Login page.");
                _logger.LogInformation(sb.ToString());

                filterContext.HttpContext.Response.Redirect("/Account/LoginPrep");   
                
                return;               
            }     

            if ( DateTime.Now > sessionCookie.LastAccessedOn.AddMinutes(30) )
            {
                StringBuilder sb = new StringBuilder("[Enrollments][IndexModel][OnPageHandlerSelected] => SessionCookie for '");
                sb.Append(appUser.UserName);
                sb.Append("' HAS EXPIRED. It was last accessed on ");
                sb.Append(sessionCookie.LastAccessedOn);

                if ( DateTime.Now > sessionCookie.LastAccessedOn.AddHours(2) )
                {
                    sb.Append(". Redirecting to Login page.");
                    _logger.LogInformation(sb.ToString());
                    filterContext.HttpContext.Response.Redirect("/Account/LoginPrep");  

                    return;

                }
                else
                {
                    sb.Append(". Redirecting to SessionTimeout page.");
                    _logger.LogInformation(sb.ToString());                    
                    filterContext.HttpContext.Response.Redirect("/Account/SessionTimeout");

                    return;
                }
            }           

            _logger.LogInformation("[Enrollments][IndexModel][OnPageHandlerSelected] => Refreshing SessinCookie for '" + appUser.UserName + "'");
            _sessionCookieService.RefreshSessionCookie(appUser.UserName);
         }
    }
}