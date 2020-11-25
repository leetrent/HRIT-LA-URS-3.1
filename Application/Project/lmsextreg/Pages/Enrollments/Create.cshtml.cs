using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc.Filters;
using lmsextreg.Data;
using lmsextreg.Models;
using lmsextreg.Constants;
using lmsextreg.Services;

using Microsoft.EntityFrameworkCore;

namespace lmsextreg.Pages.Enrollments
{
    [Authorize(Roles = "STUDENT")]
    public class CreateModel : PageModel
    {
        private readonly lmsextreg.Data.ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        private readonly IAuthorizationService _authorizationService;
        private readonly IEmailSender _emailSender;
        private readonly IEventLogService _eventLogService;
        private readonly ISessionCookieService _sessionCookieService;
        private readonly ILogger<CreateModel> _logger;         

        public CreateModel  ( 
                                lmsextreg.Data.ApplicationDbContext context,
                                UserManager<ApplicationUser> userMgr,
                                IAuthorizationService authorizationSvc,
                                IEmailSender emailSender,
                                IEventLogService eventLogSvc,
                                ISessionCookieService sessionCookieSvc,
                                ILogger<CreateModel> logger
                            )
        {
            _context = context;
            _userManager = userMgr;
            _authorizationService = authorizationSvc;
            _emailSender = emailSender;
            _eventLogService = eventLogSvc;
            _sessionCookieService = sessionCookieSvc;
            _logger = logger;             
        }

        public class InputModel
        {
            [Required]
            [Display(Name = "Program")]  
            public string LMSProgramID { get; set; }

            [Display(Name = "Remarks")]  
            public string Remarks { get; set; }
        }
        
        [BindProperty]
        public InputModel Input { get; set; }
        public SelectList ProgramSelectList { get; set; }
        public bool ShowProgramDropdown {get; set; }

        public IActionResult OnGet()
        {
            var userID = _userManager.GetUserId(User);
            
            //////////////////////////////////////////////////////////////////////////
            // Select the remaining programs that student has net as yet enrolled in
            // This is used to manage the user interface drope-down to make sure that
            // student can't enroll in the same program more than once.
            /////////////////////////////////////////////////////////////////////////
            // PostgreSQL
            /////////////////////////////////////////////////////////////////////////            
            // var sql = " SELECT * "
            //         + " FROM " + MiscConstants.DB_SCHEMA_NAME + ".\"LMSProgram\" "
            //         + " WHERE \"LMSProgramID\" "
            //         + " NOT IN "
            //         + " ( "
            //         + "   SELECT \"LMSProgramID\" "
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
                    + " ( "
                    + "   SELECT LMSProgramID "
                    + "   FROM " + MiscConstants.DB_SCHEMA_NAME + ".ProgramEnrollment "
                    + "   WHERE StudentUserId = {0} "
                    + " )";            
            ///////////////////////////////////////////////////////////////////////// 
            
            //Console.WriteLine("SQL: ");
            //Console.WriteLine(sql);
            var resultSet =  _context.LMSPrograms.FromSqlRaw(sql, userID).AsNoTracking();

            Console.WriteLine("resultSet: ");         
            Console.WriteLine(resultSet.Count());

            ProgramSelectList = new SelectList(resultSet, "LMSProgramID", "LongName");
            ShowProgramDropdown = (resultSet.Count() > 0);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            Console.WriteLine("ModelState.IsValid: " + ModelState.IsValid);
            
            if (!ModelState.IsValid)
            {
                Console.WriteLine("ModelState IS NOT valid");
                return Page();
             }
            
            Console.WriteLine("ModelState IS valid");
          
            ////////////////////////////////////////////////////////////
            // Retrieve "NONE TO PENDING" StatusTransition
            ////////////////////////////////////////////////////////////            
            var statusTransition = await _context.StatusTransitions
                                    .Where(st => st.TransitionCode == TransitionCodeConstants.NONE_TO_PENDING)
                                    .SingleOrDefaultAsync();

            ////////////////////////////////////////////////////////////
            // Create ProgramEnrollment (root class), adding newly
            // created EnrollmentHistory to the EnrollmentHistory
            // collection
            ////////////////////////////////////////////////////////////            
            var programEnrollment = new ProgramEnrollment
            {
                LMSProgramID         = Int32.Parse(Input.LMSProgramID),
                StudentUserId        = _userManager.GetUserId(User),
                UserCreated          = _userManager.GetUserId(User),
                DateCreated          = DateTime.Now,
                StatusCode           = StatusCodeConstants.PENDING,
                EnrollmentHistory    = new List<EnrollmentHistory>
                {
                    new EnrollmentHistory
                    {
                        StatusTransitionID = statusTransition.StatusTransitionID,
                        ActorUserId = _userManager.GetUserId(User),
                        ActorRemarks = Input.Remarks,
                        DateCreated = DateTime.Now

                    }
                }
            };

            /////////////////////////////////////////////////////////////////////
            // Persist ProgramEnrollment plus EnrollmentHistory to the database
            /////////////////////////////////////////////////////////////////////
            _context.ProgramEnrollments.Add(programEnrollment);
            await _context.SaveChangesAsync();

            ///////////////////////////////////////////////////////////////////
            // Log the 'ENROLLMENT_REQUSTED' event
            ///////////////////////////////////////////////////////////////////
            ApplicationUser student = await GetCurrentUserAsync();
            // _eventLogService.LogEvent(EventTypeCodeConstants.ENROLLMENT_REQUSTED, student, programEnrollment.ProgramEnrollmentID);
            _eventLogService.LogEvent(EventTypeCodeConstants.ENROLLMENT_REQUSTED, student, programEnrollment);                         

            ////////////////////////////////////////////////////////////////////////////////////
            // Send email notification to approvers who have their EmailNotify flag set to true
            ////////////////////////////////////////////////////////////////////////////////////
            IList<ProgramApprover> approverList = await _context.ProgramApprovers
                                                .Where ( pa => pa.LMSProgramID == programEnrollment.LMSProgramID && pa.EmailNotify == true)
                                                .Include ( pa => pa.Approver)
                                                .Include ( pa => pa.LMSProgram )
                                                .AsNoTracking()
                                                .ToListAsync(); 

            foreach (ProgramApprover approverObj in approverList)
            {
                string email    = approverObj.Approver.Email;
                string subject  = "Program Enrollment Request (" + approverObj.LMSProgram.LongName + ")"; 
                string message  = student.FullName + " has requested to enroll in " + approverObj.LMSProgram.LongName;
                await _emailSender.SendEmailAsync(email, subject, message);
            }
            ////////////////////////////////////////////////////////////////////////////////////
            // Send email notification to LMS Program's common inbox (if any)
            ////////////////////////////////////////////////////////////////////////////////////
            LMSProgram lmsProgram = await _context.LMSPrograms
                                    .Where(p => p.LMSProgramID == programEnrollment.LMSProgramID && p.CommonInbox != null)
                                    .AsNoTracking()
                                    .SingleOrDefaultAsync();
            if ( lmsProgram != null 
                    && String.IsNullOrEmpty(lmsProgram.CommonInbox) == false )
            {
                string email = lmsProgram.CommonInbox;
                string subject  = "Program Enrollment Request (" + lmsProgram.LongName + ")";
                string message  = student.FullName + " has requested to enroll in " + lmsProgram.LongName;
                await _emailSender.SendEmailAsync(email, subject, message);
            }               

            /////////////////////////////////////////////////////////////////
            // Redirect to Enrollmentl Index Page
            /////////////////////////////////////////////////////////////////            
            return RedirectToPage("./Index");
        }        

        private Task<ApplicationUser> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);

        public override void OnPageHandlerSelected(PageHandlerSelectedContext filterContext)
        {
            string tagName = "[Enrollments][CreateModel][OnPageHandlerSelected]";

            ApplicationUser appUser = _userManager.GetUserAsync(HttpContext.User).Result;
            
            string pageRedirect = _sessionCookieService.DeterminePageRedirect(appUser.UserName, tagName);
            if ( pageRedirect != null )
            {
                filterContext.HttpContext.Response.Redirect(pageRedirect);
                return;
            }
            
            _logger.LogInformation(tagName + " => Refreshing SessinCookie for '" + appUser.UserName + "'");
            _sessionCookieService.RefreshSessionCookie(appUser.UserName);
        }
    }
}