using System;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using lmsextreg.Data;
using lmsextreg.Models;
using lmsextreg.Constants;
using lmsextreg.Services;
using lmsextreg.Utils;

namespace lmsextreg.Pages.Approvals
{
    [Authorize(Roles = "APPROVER")]
    public class IndexModel: PageModel
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ISessionCookieService _sessionCookieService;
        private readonly ILogger<IndexModel> _logger;


        public IndexModel(  lmsextreg.Data.ApplicationDbContext dbCntx,
                            UserManager<ApplicationUser> usrMgr,
                            ISessionCookieService sessionCookieSvc,
                            ILogger<IndexModel> logger)
        {
            _dbContext = dbCntx;
            _userManager = usrMgr;
            _sessionCookieService = sessionCookieSvc;
            _logger = logger;
        }

        public string ProgramName { get; set; }
        public IList<ProgramEnrollment> ProgramEnrollment { get;set; }

        public IList<EnrollmentStatusCount> EnrollmentStatusCounts{ get; set; }

        public ApplicationUser LoggedInUser {get;set;}

        public string PENDING   = StatusCodeConstants.PENDING;
        public string WITHDRAWN = StatusCodeConstants.WITHDRAWN;
        public string APPROVED  = StatusCodeConstants.APPROVED;
        public string DENIED    = StatusCodeConstants.DENIED;
        public string REVOKED   = StatusCodeConstants.REVOKED;

        public SelectList StatusSelectList { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Display(Name = "Enrollment Status")]  
            public string StatusCode { get; set; }
        }
        
        public async Task OnGetAsync(string statusCode = "PENDING")
        {

            string logSnippet = new StringBuilder("[")
                    .Append(DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"))
                    .Append("][Approvals][OnGetAsync][HttpGet] => ")
                    .ToString();
            Console.WriteLine(logSnippet + $"(statusCode): '{statusCode}'");

            LoggedInUser = await GetCurrentUserAsync();

            if ( User.IsInRole(RoleConstants.APPROVER))
            {
                var loggedInUserID = _userManager.GetUserId(User);

                ///////////////////////////////////////////////////////////////
                // Make sure that the logged-in user with the role of approver
                // is authorized to approve /deny /revoke enrollment
                // requests for this particular LMS Program.
                /////////////////////////////////////////////////////////////////////////

                // var sql = " SELECT * "
                //         + " FROM " + MiscConstants.DB_SCHEMA_NAME +  ".ProgramEnrollment "
                //         + " WHERE LMSProgramID " 
                //         + " IN "
                //         + " ( "
                //         + "   SELECT LMSProgramID "
                //         + "   FROM " + MiscConstants.DB_SCHEMA_NAME +  ".ProgramApprover "
                //         + "   WHERE ApproverUserId = {0} "
                //         + " ) "
                //         + " AND StatusCode = {1}";

                var sql = " SELECT * "
                        + " FROM " + MiscConstants.DB_SCHEMA_NAME +  ".ProgramEnrollment "
                        + " WHERE LMSProgramID " 
                        + " IN "
                        + " ( "
                        + "   SELECT LMSProgramID "
                        + "   FROM " + MiscConstants.DB_SCHEMA_NAME +  ".ProgramApprover "
                        + "   WHERE ApproverUserId = {0} "
                        + " ) ";

                    Console.WriteLine(logSnippet + "BEGIN SQL:");
                    Console.WriteLine(sql);        
                    Console.WriteLine(logSnippet + ":END SQL:");                

                    // ProgramEnrollment  = await _dbContext.ProgramEnrollments
                    //                     .FromSql(sql, loggedInUserID, statusCode)
                    //                     .Include( pe =>  pe.LMSProgram)
                    //                     .Include ( pe => pe.Student).ThenInclude(s => s.Agency)
                    //                     .Include( pe => pe.EnrollmentStatus)
                    //                     .Include( pe => pe.Approver)
                    //                     .OrderBy( pe => pe.EnrollmentStatus.StatusCode)
                    //                         .ThenBy(pe => pe.Student.LastName)
                    //                         .ThenBy(pe => pe.Student.FirstName)
                    //                     .ToListAsync();

                    ProgramApprover programApprover = await _dbContext.ProgramApprovers
                                                    .AsNoTracking()
                                                    .Where(pa => pa.ApproverUserId == loggedInUserID)
                                                    .Include(pa => pa.LMSProgram)
                                                    .SingleOrDefaultAsync();
                    
                    ProgramName = programApprover.LMSProgram.LongName;


                    ProgramEnrollment  = await _dbContext.ProgramEnrollments
                                        .FromSqlRaw(sql, loggedInUserID)
                                        .Include( pe =>  pe.LMSProgram)
                                        .Include ( pe => pe.Student).ThenInclude(s => s.Agency)
                                        .Include( pe => pe.EnrollmentStatus)
                                        .Include( pe => pe.Approver)
                                        .OrderBy( pe => pe.EnrollmentStatus.StatusCode)
                                            .ThenBy(pe => pe.Student.LastName)
                                            .ThenBy(pe => pe.Student.FirstName)
                                        .ToListAsync();


                    //ProgramEnrollment = ProgramEnrollmentUtil.SortByStatusCode(ProgramEnrollment);
                    IQueryable<EnrollmentStatus> enrollmentStatusList =  _dbContext.EnrollmentStatuses
                                                                        .AsNoTracking()
                                                                        .Where(es => es.DisplayOrder > 0)
                                                                        .OrderBy(es => es.DisplayOrder);
                    EnrollmentStatusCounts = EnrollmentStatusUtil.GroupByStatusCode(ProgramEnrollment, enrollmentStatusList);
                    ProgramEnrollment = ProgramEnrollmentUtil.FilterByStatusCode(ProgramEnrollment, statusCode);
                    StatusSelectList  = new SelectList(_dbContext.EnrollmentStatuses.OrderBy(a => a.StatusCode), "StatusCode", "StatusLabel", statusCode);


                    string statusChangeModalTitle = HttpContext.Session.GetString(ApproverConstants.STATUS_CHANGE_MODAL_TITLE);
                    string statusChangeModalText  = HttpContext.Session.GetString(ApproverConstants.STATUS_CHANGE_MODAL_TEXT);

                    Console.WriteLine(logSnippet + $"(statusChangeModalTitle): '{statusChangeModalTitle}'");
                    Console.WriteLine(logSnippet + $"(statusChangeModalText).: '{statusChangeModalText}'");

                    if ( String.IsNullOrEmpty(statusChangeModalTitle) == false
                            && String.IsNullOrWhiteSpace(statusChangeModalTitle) == false
                            && String.IsNullOrEmpty(statusChangeModalText) == false
                            && String.IsNullOrWhiteSpace(statusChangeModalText) == false )
                    {
                        ViewData[ApproverConstants.STATUS_CHANGE_MODAL_TITLE] = statusChangeModalTitle;
                        ViewData[ApproverConstants.STATUS_CHANGE_MODAL_TEXT]  = statusChangeModalText;
                    }

                    HttpContext.Session.SetString(ApproverConstants.STATUS_CHANGE_MODAL_TITLE, "");
                    HttpContext.Session.SetString(ApproverConstants.STATUS_CHANGE_MODAL_TEXT, "");
            }
        }
        private Task<ApplicationUser> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);

        public override void OnPageHandlerSelected(PageHandlerSelectedContext filterContext)
        {
            string tagName = "[Approvals][IndexModel][OnPageHandlerSelected]";

            ApplicationUser appUser = _userManager.GetUserAsync(HttpContext.User).Result;
            
            string pageRedirect = _sessionCookieService.DeterminePageRedirect(appUser.UserName, tagName);
            if ( pageRedirect != null )
            {
                filterContext.HttpContext.Response.Redirect(pageRedirect);
                return;
            }
            
            _logger.LogDebug(tagName + " => Refreshing SessinCookie for '" + appUser.UserName + "'");
            _sessionCookieService.RefreshSessionCookie(appUser.UserName);
        }

        public IActionResult OnPostAsync(string returnUrl = null)
        {
            string logSnippet = new StringBuilder("[")
                    .Append(DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"))
                    .Append("][Approvals][Index][HttpPost] => ")
                    .ToString();

            System.Console.WriteLine(logSnippet + $"(Input.StatusCode): {Input.StatusCode}");

            return RedirectToAction(nameof(OnGetAsync), new {@statusCode = Input.StatusCode});
        }
    }
}