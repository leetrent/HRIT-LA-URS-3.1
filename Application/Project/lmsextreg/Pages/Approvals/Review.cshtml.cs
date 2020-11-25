using System;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc.Filters;
using lmsextreg.Constants;
using lmsextreg.Data;
using lmsextreg.Models;
using lmsextreg.Services;

namespace lmsextreg.Pages.Approvals
{
    [Authorize(Roles = "APPROVER")]
    public class ReviewModel: PageModel
    {
         private readonly ApplicationDbContext _dbContext;
         private readonly UserManager<ApplicationUser> _userManager;
         private readonly IEmailSender _emailSender;
         private readonly IEventLogService _eventLogService;
        private readonly ISessionCookieService _sessionCookieService;
        private readonly ILogger<ReviewModel> _logger;         

        public ReviewModel  
        (
            lmsextreg.Data.ApplicationDbContext dbCntx, 
            UserManager<ApplicationUser> usrMgr,
            IEmailSender emailSndr,
            IEventLogService eventLogSvc,
            ISessionCookieService sessionCookieSvc,
            ILogger<ReviewModel> logger
        )
        {
            _dbContext = dbCntx;
            _userManager = usrMgr;
            _emailSender = emailSndr;
            _eventLogService = eventLogSvc;
            _sessionCookieService = sessionCookieSvc;
            _logger = logger;            
        }         

        public class InputModel
        {
            [Display(Name = "Remarks")]  
            public string Remarks { get; set; }
        }   

        [BindProperty]
        public InputModel Input { get; set; }
        public ProgramEnrollment ProgramEnrollment { get; set; }
        public IList<EnrollmentHistory> EnrollmentHistory { get;set; }
        public bool ShowReviewForm { get; set; }
        public bool ShowApproveButton {get; set; }
        public bool ShowDenyButton {get; set; }
        public bool ShowRevokeButton {get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            ////////////////////////////////////////////////////////////
            // Step #1:
            // Check to see if records exists
            ////////////////////////////////////////////////////////////
            ProgramEnrollment = await _dbContext.ProgramEnrollments
                              .Where(pe => pe.ProgramEnrollmentID == id) 
                              .SingleOrDefaultAsync();     

            ////////////////////////////////////////////////////////////
            // Return "Not Found" if record doesn't exist
            ////////////////////////////////////////////////////////////
            if (ProgramEnrollment == null)
            {
                return NotFound();
            }                                     

            ////////////////////////////////////////////////////////////
            // Step #2:
            // Now that record exists, make sure that the logged-in user
            // is authorized to edit (approver/deny) enrollment
            // applications for this particular LMS Program.
            ////////////////////////////////////////////////////////////
            // PostgreSQL
            /////////////////////////////////////////////////////////////////////////            
            // var sql = " SELECT * "
            //         + "   FROM " + MiscConstants.DB_SCHEMA_NAME + ".\"ProgramEnrollment\" "
            //         + "  WHERE  \"ProgramEnrollmentID\" = {0} "
            //         + "    AND  \"LMSProgramID\" " 
            //         + "     IN "
            //         + "      ( "
            //         + "        SELECT \"LMSProgramID\" "
            //         + "          FROM " + MiscConstants.DB_SCHEMA_NAME + ".\"ProgramApprover\" "
            //         + "         WHERE \"ApproverUserId\" = {1} "
            //         + "      ) ";
            /////////////////////////////////////////////////////////////////////////
            // MySQL
            /////////////////////////////////////////////////////////////////////////      
            var sql = " SELECT * "
                    + "   FROM " + MiscConstants.DB_SCHEMA_NAME + ".ProgramEnrollment "
                    + "  WHERE  ProgramEnrollmentID = {0} "
                    + "    AND  LMSProgramID " 
                    + "     IN "
                    + "      ( "
                    + "        SELECT LMSProgramID "
                    + "          FROM " + MiscConstants.DB_SCHEMA_NAME + ".ProgramApprover "
                    + "         WHERE ApproverUserId = {1} "
                    + "      ) ";
            /////////////////////////////////////////////////////////////////////////  
            ProgramEnrollment = null;
            ProgramEnrollment = await _dbContext.ProgramEnrollments
                                .FromSqlRaw(sql, id, _userManager.GetUserId(User))
                                .Include(pe => pe.LMSProgram)
                                .Include(pe => pe.Student)
                                    .ThenInclude(s => s.SubAgency)
                                    .ThenInclude(sa => sa.Agency)
                                .Include(pe => pe.EnrollmentStatus)
                                .SingleOrDefaultAsync();

            EnrollmentHistory = await _dbContext.EnrollmentHistories
                                    .Where(eh => eh.ProgramEnrollmentID == ProgramEnrollment.ProgramEnrollmentID)
                                    .Include(eh => eh.Actor)
                                    .Include(eh => eh.StatusTransition)
                                    .OrderBy(eh => eh.EnrollmentHistoryID)
                                    .ToListAsync();
                                    
            /////////////////////////////////////////////////////////////
            // We already know that record exists from Step #1 so if we
            // get a "Not Found" in Step #2, we know it's because the 
            // logged-in user is not authorized to edit (approve/deny)
            // enrollment applications for this LMS Program.
            /////////////////////////////////////////////////////////////
            if (ProgramEnrollment == null)
            {
                return Unauthorized();
            }

            if ( ProgramEnrollment.StatusCode == StatusCodeConstants.PENDING )
            {
                ShowReviewForm      = true;
                ShowApproveButton   = true;
                ShowDenyButton      = true;
                ShowRevokeButton    = false;
            }
            if ( ProgramEnrollment.StatusCode == StatusCodeConstants.APPROVED )
            {
                ShowReviewForm      = true;
                ShowApproveButton   = false;
                ShowDenyButton      = false;
                ShowRevokeButton    = true;
            }   
            if ( ProgramEnrollment.StatusCode == StatusCodeConstants.DENIED )
            {
                ShowReviewForm      = true;
                ShowApproveButton   = true;
                ShowDenyButton      = false;
                ShowRevokeButton    = false;
            }   
            if ( ProgramEnrollment.StatusCode == StatusCodeConstants.REVOKED )
            {
                ShowReviewForm      = false;
                ShowApproveButton   = false;
                ShowDenyButton      = false;
                ShowRevokeButton    = false;
            }                                    

            return Page();                              
        }

        public async Task<IActionResult> OnPostApproveAsync(int id)
        {
            return await this.OnPostAsync
            (
                id, 
                StatusCodeConstants.APPROVED, 
                TransitionCodeConstants.PENDING_TO_APPROVED
            );
        }

        public async Task<IActionResult> OnPostDenyAsync(int id)
        {
            ///////////////////////////////////////////////////////////////
            // Remarks are required when Enrollment Request is DENIED
            ///////////////////////////////////////////////////////////////
            if ( String.IsNullOrEmpty(Input.Remarks))
            {
                ModelState.AddModelError("ApproverRemarks", "Remarks are required when enrollment request has been denied");

                ////////////////////////////////////////////////////////////////////////////////////////////////
                // Rebuild page 
                ////////////////////////////////////////////////////////////////////////////////////////////////
                ProgramEnrollment = await this.retrieveProgramEnrollment(this.createAuthorizationSQL(), id);
                EnrollmentHistory = await this.retrieveEnrollmentHistory(ProgramEnrollment.ProgramEnrollmentID); 
                ShowReviewForm      = true;
                ShowApproveButton   = true;
                ShowDenyButton      = true;
                ShowRevokeButton    = false;
                return Page();
            }

            return await this.OnPostAsync
            (
                id, 
                StatusCodeConstants.DENIED, 
                TransitionCodeConstants.PENDING_TO_DENIED
            );
        }
        public async Task<IActionResult> OnPostRevokeAsync(int id)
        {
            return await this.OnPostAsync
            (
                id, 
                StatusCodeConstants.REVOKED, 
                TransitionCodeConstants.APPROVED_TO_REVOKED
            );
        }

        public async Task<IActionResult> OnPostAsync(int programEnrollmentID, string statusCode, string statusTransitionCode)
        {
            ////////////////////////////////////////////////////////////
            // Step #1:
            // Check to see if records exists
            ////////////////////////////////////////////////////////////
            var lvProgramEnrollment = await _dbContext.ProgramEnrollments
                                        .Where(pe => pe.ProgramEnrollmentID == programEnrollmentID) 
                                        .AsNoTracking()
                                        .SingleOrDefaultAsync();     

            ////////////////////////////////////////////////////////////
            // Return "Not Found" if record doesn't exist
            ////////////////////////////////////////////////////////////
            if (lvProgramEnrollment == null)
            {
                _logger.LogInformation("[Approvals][ReviewModel][OnPostAsync] => ProgramEnrollment NOT FOUND ProgramEnrollmentID '" + programEnrollmentID + "'");
                return NotFound();
            } 

            ////////////////////////////////////////////////////////////
            // Step #2:
            // Now that record exists, make sure that the logged-in user
            // is authorized to edit (approver/deny) enrollment
            // applications for this particular LMS Program.
            //////////////////////////////////////////////////////////////////////////
            // PostgreSQL
            /////////////////////////////////////////////////////////////////////////              
            // var sql = " SELECT * "
            //         + "   FROM " + MiscConstants.DB_SCHEMA_NAME + ".\"ProgramEnrollment\" "
            //         + "  WHERE  \"ProgramEnrollmentID\" = {0} "
            //         + "    AND  \"LMSProgramID\" " 
            //         + "     IN "
            //         + "      ( "
            //         + "        SELECT \"LMSProgramID\" "
            //         + "          FROM " + MiscConstants.DB_SCHEMA_NAME + ".\"ProgramApprover\" "
            //         + "         WHERE \"ApproverUserId\" = {1} "
            //         + "      ) ";
            /////////////////////////////////////////////////////////////////////////
            // MySQL
            ///////////////////////////////////////////////////////////////////////// 
            var sql = " SELECT * "
                    + "   FROM " + MiscConstants.DB_SCHEMA_NAME + ".ProgramEnrollment "
                    + "  WHERE  ProgramEnrollmentID = {0} "
                    + "    AND  LMSProgramID " 
                    + "     IN "
                    + "      ( "
                    + "        SELECT LMSProgramID "
                    + "          FROM " + MiscConstants.DB_SCHEMA_NAME + ".ProgramApprover "
                    + "         WHERE ApproverUserId = {1} "
                    + "      ) ";
            /////////////////////////////////////////////////////////////////////////

            lvProgramEnrollment = null;
            lvProgramEnrollment = await _dbContext.ProgramEnrollments
                                .FromSqlRaw(sql, programEnrollmentID, _userManager.GetUserId(User))
                                .Include(pe => pe.LMSProgram)
                                .Include(pe => pe.EnrollmentHistory)
                                .SingleOrDefaultAsync();

            /////////////////////////////////////////////////////////////
            // We already know that record exists from Step #1 so if we
            // get a "Not Found" in Step #2, we know it's because the 
            // logged-in user is not authorized to edit (approve/deny)
            // enrollment applications for this LMS Program.
            /////////////////////////////////////////////////////////////
            if (lvProgramEnrollment == null)
            {
                return Unauthorized();
            }            

            ////////////////////////////////////////////////////////////
            // Retrieve the correct StatusTransition
            ////////////////////////////////////////////////////////////            
            var lvStatusTranstion = await _dbContext.StatusTransitions
                                    .Where(st => st.TransitionCode == statusTransitionCode)
                                    .AsNoTracking()
                                    .SingleOrDefaultAsync();
            
            ////////////////////////////////////////////////////////////////
            // Create EnrollmentHistory using the correct StatusTranistion
            ////////////////////////////////////////////////////////////////            
            var lvEnrollmentHistory = new EnrollmentHistory()
            {
                    StatusTransitionID = lvStatusTranstion.StatusTransitionID,
                    ActorUserId = _userManager.GetUserId(User),
                    ActorRemarks = Input.Remarks,
                    DateCreated = DateTime.Now
            };

            ////////////////////////////////////////////////////////////
            // Instantiate EnrollmentHistory, if necessary
            ////////////////////////////////////////////////////////////
            if ( lvProgramEnrollment.EnrollmentHistory == null) 
            {
                lvProgramEnrollment.EnrollmentHistory = new List<EnrollmentHistory>();
            }

            ///////////////////////////////////////////////////////////////////
            // Add newly created EnrollmentHistory with StatusTransition  
            // to ProgramEnrollment's EnrollmentHistory Collection
            ///////////////////////////////////////////////////////////////////            
            lvProgramEnrollment.EnrollmentHistory.Add(lvEnrollmentHistory);

            /////////////////////////////////////////////////////////////////
            // Update ProgramEnrollment Record with
            //  1. EnrollmentStatus of "APPROVED"
            //  2. ApproverUserId (logged-in user)
            //  3. EnrollmentHistory (PENDING TO APPROVED)
            /////////////////////////////////////////////////////////////////
            string logSnippet = new StringBuilder("[")
                    .Append(DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"))
                    .Append("][")
                    .Append(_userManager.GetUserId(User))
                    .Append("][Approver][Review][HttpPost] => ")
                    .ToString();
            
            Console.WriteLine(logSnippet + $"(lvProgramEnrollment.LMSProgram == null)....: {lvProgramEnrollment.LMSProgram == null}");
            Console.WriteLine(logSnippet + $"(lvProgramEnrollment.LMSProgram.ShortName)..: {lvProgramEnrollment.LMSProgram.ShortName}");
            Console.WriteLine(logSnippet + $"(lvProgramEnrollment.LMSProgram.ExpiryDays).: {lvProgramEnrollment.LMSProgram.ExpiryDays}");

            lvProgramEnrollment.StatusCode = statusCode;
            lvProgramEnrollment.DateLastUpdated = DateTime.Now;
            
            
            if ( lvProgramEnrollment.StatusCode.Equals(StatusCodeConstants.APPROVED))
            {
                lvProgramEnrollment.DateExpired = lvProgramEnrollment.DateLastUpdated.AddDays(lvProgramEnrollment.LMSProgram.ExpiryDays);
            }
            else if (lvProgramEnrollment.StatusCode.Equals(StatusCodeConstants.REVOKED))
            {
                lvProgramEnrollment.DateExpired = DateTime.Now;
            }
            else
            {
                lvProgramEnrollment.DateExpired = null;
            }

            Console.WriteLine(logSnippet + $"(lvProgramEnrollment.DateLastUpdated)..: {lvProgramEnrollment.DateLastUpdated}");
            Console.WriteLine(logSnippet + $"(lvProgramEnrollment.StatusCode).......: {lvProgramEnrollment.StatusCode}");
            Console.WriteLine(logSnippet + $"(lvProgramEnrollment.DateExpired)......: {lvProgramEnrollment.DateExpired}");

            lvProgramEnrollment.ApproverUserId = _userManager.GetUserId(User);
            _dbContext.ProgramEnrollments.Update(lvProgramEnrollment);
            await _dbContext.SaveChangesAsync();

            //////////////////////////////////////////////////////////////////////////////
            // Retrieve 'User as Approver' for event logging
            //////////////////////////////////////////////////////////////////////////////
            ApplicationUser approver = await GetCurrentUserAsync();
            
            //////////////////////////////////////////////////////////////////
            // Determine EventType StatusCode
            //////////////////////////////////////////////////////////////////
            string eventTypeCode = null;
            if ( statusCode.Equals(StatusCodeConstants.APPROVED) )
            {
                eventTypeCode = EventTypeCodeConstants.ENROLLMENT_APPROVED;
            }
            if ( statusCode.Equals(StatusCodeConstants.DENIED) )
            {
                eventTypeCode = EventTypeCodeConstants.ENROLLMENT_DENIED;
            }     
            if ( statusCode.Equals(StatusCodeConstants.REVOKED) )
            {
                eventTypeCode = EventTypeCodeConstants.ENROLLMENT_REVOKED;
            }                             

            /////////////////////////////////////////////////////////////////////////////////////////////
            // Log the ENROLLMENT <APPROVED || DENIED || REVOKED> event
            /////////////////////////////////////////////////////////////////////////////////////////////
            //_eventLogService.LogEvent(eventTypeCode, approver, lvProgramEnrollment.ProgramEnrollmentID); 
            _eventLogService.LogEvent(eventTypeCode, approver, lvProgramEnrollment); 

            /////////////////////////////////////////////////////////////////////
            // Send email notification to student, advising him/her of
            // program enrollment status
            /////////////////////////////////////////////////////////////////////
            lvProgramEnrollment = null;
            lvProgramEnrollment = await _dbContext.ProgramEnrollments
                                .Where      ( pe => pe.ProgramEnrollmentID == programEnrollmentID)
                                .Include    ( pe => pe.LMSProgram)
                                .Include    ( pe => pe.Student)
                                .Include    ( pe => pe.EnrollmentStatus)
                                .AsNoTracking()
                                .SingleOrDefaultAsync(); 

            ApplicationUser student =  lvProgramEnrollment.Student;
            string email = student.Email;
            string subject  = lvProgramEnrollment.LMSProgram.LongName 
                            + " Enrollment Status";
            string message  = "Your request to enroll in the " 
                            + lvProgramEnrollment.LMSProgram.LongName 
                            + " has been " 
                            + lvProgramEnrollment.EnrollmentStatus.StatusLabel
                            + ".";

            ////////////////////////////////////////////////////////////////
            // SET BANNER THAT WILL BE DISPLAYED ON THE APPROVER INDEX PAGE
            ////////////////////////////////////////////////////////////////
            StringBuilder bannerMsg = new StringBuilder();
            bannerMsg.Append("Request by ");
            bannerMsg.Append(lvProgramEnrollment.Student.FullName); 
            bannerMsg.Append(" to enroll in ");
            bannerMsg.Append(lvProgramEnrollment.LMSProgram.LongName); 
            bannerMsg.Append(" has been ");
            bannerMsg.Append(lvProgramEnrollment.EnrollmentStatus.StatusLabel);
            bannerMsg.Append(".");
            HttpContext.Session.SetString(ApproverConstants.STATUS_CHANGE_MODAL_TITLE, lvProgramEnrollment.EnrollmentStatus.StatusCode);
            HttpContext.Session.SetString(ApproverConstants.STATUS_CHANGE_MODAL_TEXT,  bannerMsg.ToString());

            ////////////////////////////////////////////////////////////////
            // BUILD EMAIL MESSAGE TO STUDENT
            ////////////////////////////////////////////////////////////////           
            if  ( StatusCodeConstants.APPROVED.Equals(lvProgramEnrollment.EnrollmentStatus.StatusCode)) 
            {
                message += "You will receive an additional email from the GSA Learning Academy "
                        + "to finish setting up your account in the GSA Learning Academy "
                        + "and begin the training program";
            }
            if  ( StatusCodeConstants.DENIED.Equals(lvProgramEnrollment.EnrollmentStatus.StatusCode))          
            {
                message += " <a href='" + MiscConstants.LOGIN_URL + "'>Log-in</a> "
                        + "to the training registration system for more information "
                        + "regarding the denial and request to reenroll in the program..";
            }  
            if  ( StatusCodeConstants.REVOKED.Equals(lvProgramEnrollment.EnrollmentStatus.StatusCode))          
            {
                message += " <a href='" + MiscConstants.LOGIN_URL + "'>Log-in</a> "
                        + "to the training registration system for more information "
                        + "regarding the revocation and request to reenroll in the program..";
            }  

            await _emailSender.SendEmailAsync(email, subject, message);

            /////////////////////////////////////////////////////////////////
            // Redirect to Approval Index Page
            /////////////////////////////////////////////////////////////////
            return RedirectToPage("./Index");          
        }

        private string createAuthorizationSQL() 
        {
            //////////////////////////////////////////////////////////////////////////
            // PostgreSQL
            ///////////////////////////////////////////////////////////////////////// 
            // return    " SELECT * "
            //         + "   FROM " + MiscConstants.DB_SCHEMA_NAME + ".\"ProgramEnrollment\" "
            //         + "  WHERE  \"ProgramEnrollmentID\" = {0} "
            //         + "    AND  \"LMSProgramID\" " 
            //         + "     IN "
            //         + "      ( "
            //         + "        SELECT \"LMSProgramID\" "
            //         + "          FROM " + MiscConstants.DB_SCHEMA_NAME + ".\"ProgramApprover\" "
            //         + "         WHERE \"ApproverUserId\" = {1} "
            //         + "      ) ";
            //////////////////////////////////////////////////////////////////////////
            // MySQL
            /////////////////////////////////////////////////////////////////////////  
            return    " SELECT * "
                    + "   FROM " + MiscConstants.DB_SCHEMA_NAME + ".ProgramEnrollment "
                    + "  WHERE  ProgramEnrollmentID = {0} "
                    + "    AND  LMSProgramID " 
                    + "     IN "
                    + "      ( "
                    + "        SELECT LMSProgramID "
                    + "          FROM " + MiscConstants.DB_SCHEMA_NAME + ".ProgramApprover "
                    + "         WHERE ApproverUserId = {1} "
                    + "      ) ";              
            /////////////////////////////////////////////////////////////////////////                 
        }
        private async Task<ProgramEnrollment> retrieveProgramEnrollment(string sql, int programEnrollmentID)   
        {
            return await _dbContext.ProgramEnrollments
                            .FromSqlRaw(sql, programEnrollmentID, _userManager.GetUserId(User))
                            .Include(pe => pe.LMSProgram)
                            .Include(pe => pe.Student)
                                .ThenInclude(s => s.SubAgency)
                                .ThenInclude(sa => sa.Agency)
                            .Include(pe => pe.EnrollmentStatus)
                            .SingleOrDefaultAsync();
        }     

       private async Task<List<EnrollmentHistory>> retrieveEnrollmentHistory(int programEnrollmentID)   
        {
            return await _dbContext.EnrollmentHistories
                                    .Where(eh => eh.ProgramEnrollmentID == programEnrollmentID)
                                    .Include(eh => eh.Actor)
                                    .Include(eh => eh.StatusTransition)
                                    .OrderBy(eh => eh.EnrollmentHistoryID)
                                    .ToListAsync();
        }
        private Task<ApplicationUser> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);   

        public override void OnPageHandlerSelected(PageHandlerSelectedContext filterContext)
        {
            string tagName = "[Approvals][ReviewModel][OnPageHandlerSelected]";

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
    }
}