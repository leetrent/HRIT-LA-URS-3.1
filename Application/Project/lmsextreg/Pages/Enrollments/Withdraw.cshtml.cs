using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc.Filters;
using lmsextreg.Data;
using lmsextreg.Models;
using lmsextreg.Constants;
using lmsextreg.Services;

namespace lmsextreg.Pages.Enrollments
{
    [Authorize(Roles = "STUDENT")]
    public class WithdrawModel : PageModel
    {
        private readonly lmsextreg.Data.ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _emailSender;
        private readonly IEventLogService _eventLogService;
        private readonly ISessionCookieService _sessionCookieService;
        private readonly ILogger<WithdrawModel> _logger;         

        public WithdrawModel(   lmsextreg.Data.ApplicationDbContext context, 
                                UserManager<ApplicationUser> userMgr,
                                IEmailSender emailSender,
                                IEventLogService eventLogSvc,
                                ISessionCookieService sessionCookieSvc,
                                ILogger<WithdrawModel> logger
                            )
        {
            _context = context;
            _userManager = userMgr;
            _emailSender = emailSender;
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
        public ProgramEnrollment ProgramEnrollment { get; set; }
        
        [BindProperty]
        public InputModel Input { get; set; }

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
            ProgramEnrollment = await _context.ProgramEnrollments
                                .Include(p => p.LMSProgram)
                                .SingleOrDefaultAsync(m => m.ProgramEnrollmentID == id);

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
            // is authorized to withdraw this program enrollment
            ////////////////////////////////////////////////////////////
            var loggedInUserID = _userManager.GetUserId(User);
            ProgramEnrollment = null;
            ProgramEnrollment = await _context.ProgramEnrollments
                                .Where(pe => pe.StudentUserId == loggedInUserID && pe.ProgramEnrollmentID == id) 
                                .Include(p => p.LMSProgram)
                                .SingleOrDefaultAsync();                   
                  
            /////////////////////////////////////////////////////////////
            // We already know that record exists from Step #1 so if we
            // get a "Not Found" in Step #2, we know it's because the 
            // logged-in user is not authorized to withdraw this
            // program enrollment.
            /////////////////////////////////////////////////////////////
            if (ProgramEnrollment == null)
            {
                return Unauthorized();
            }
            
            //////////////////////////////////////////////////////////////////////////////////
            // If we get this far, then record was found and user is authorized to access it
            //////////////////////////////////////////////////////////////////////////////////            
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            ////////////////////////////////////////////////////////////
            // Step #1:
            // Check to see if records exists
            ////////////////////////////////////////////////////////////            
            var lvProgramEnrollment = await _context.ProgramEnrollments
                                        .Include(p => p.LMSProgram)
                                        .SingleOrDefaultAsync(m => m.ProgramEnrollmentID == id);

            ////////////////////////////////////////////////////////////
            // Return "Not Found" if record doesn't exist
            ////////////////////////////////////////////////////////////
            if (lvProgramEnrollment == null)
            {
                return NotFound();
            } 

            ////////////////////////////////////////////////////////////
            // Step #2:
            // Now that record exists, make sure that the logged-in user
            // is authorized to withdraw this program enrollment
            ////////////////////////////////////////////////////////////
            var loggedInUserID = _userManager.GetUserId(User);
            lvProgramEnrollment = null;
            lvProgramEnrollment = await _context.ProgramEnrollments
                                    .Where(pe => pe.StudentUserId == loggedInUserID && pe.ProgramEnrollmentID == id) 
                                    .Include(p => p.LMSProgram)
                                    .SingleOrDefaultAsync();                   
                  
            /////////////////////////////////////////////////////////////
            // We already know that record exists from Step #1 so if we
            // get a "Not Found" in Step #2, we know it's because the 
            // logged-in user is not authorized to withdraw this
            // program enrollment.
            /////////////////////////////////////////////////////////////
            if (lvProgramEnrollment == null)
            {
                return Unauthorized();
            }

            ////////////////////////////////////////////////////////////
            // Retrieve the correct StatusTransition
            ////////////////////////////////////////////////////////////   
            StatusTransition lvStatusTranstion  = null;

            //////////////////////////////////////////////////////////////////////            
            // STATUS TRANSITION: PENDING TO WITHDRAEN
            //////////////////////////////////////////////////////////////////////
            if (lvProgramEnrollment.StatusCode.Equals(StatusCodeConstants.PENDING))
            {
                lvStatusTranstion = await _context.StatusTransitions
                                    .Where(st => st.TransitionCode == TransitionCodeConstants.PENDING_TO_WITHDRAWN)
                                    .SingleOrDefaultAsync();
            }
            //////////////////////////////////////////////////////////////////////            
            // STATUS TRANSITION: APPROVED TO WITHDRAEN
            //////////////////////////////////////////////////////////////////////
            if (lvProgramEnrollment.StatusCode.Equals(StatusCodeConstants.APPROVED))
            {
                lvStatusTranstion = await _context.StatusTransitions
                                    .Where(st => st.TransitionCode == TransitionCodeConstants.APPROVED_TO_WITHDRAWN)
                                    .SingleOrDefaultAsync();
            }

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
            // Instantiate EnrollmentHistory Collection, if necessary
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
            //  1. EnrollmentStatus of "WITHDRAWN"
            //  2. ApproverUserId (logged-in user)
            //  3. EnrollmentHistory (PENDING TO WiTHDRAWN)
            /////////////////////////////////////////////////////////////////
            lvProgramEnrollment.StatusCode = StatusCodeConstants.WITHDRAWN;
            //lvProgramEnrollment.ApproverUserId = _userManager.GetUserId(User);
            _context.ProgramEnrollments.Update(lvProgramEnrollment);
            await _context.SaveChangesAsync();

            //////////////////////////////////////////////////////////////////////////////
            // Retrieve 'User as Student' for event logging and email notification purposes
            //////////////////////////////////////////////////////////////////////////////
            ApplicationUser student = await GetCurrentUserAsync();
            
            /////////////////////////////////////////////////////////////////////////////////
            // Log the 'ENROLLMENT_WITHDRAWN' event
            ////////////////////////////////////////////////////////////////////////////////
            //_eventLogService.LogEvent(EventTypeCodeConstants.ENROLLMENT_WITHDRAWN, student, lvProgramEnrollment.ProgramEnrollmentID); 
            _eventLogService.LogEvent(EventTypeCodeConstants.ENROLLMENT_WITHDRAWN, student, lvProgramEnrollment); 
            ////////////////////////////////////////////////////////////////////////////////////
            // Send email notification to approvers who have their EmailNotify flag set to true
            ////////////////////////////////////////////////////////////////////////////////////
            IList<ProgramApprover> approverList = await _context.ProgramApprovers
                                                .Where ( pa => pa.LMSProgramID == lvProgramEnrollment.LMSProgramID && pa.EmailNotify == true)
                                                .Include ( pa => pa.Approver)
                                                .Include ( pa => pa.LMSProgram )
                                                .AsNoTracking()
                                                .ToListAsync(); 

            foreach (ProgramApprover approverObj in approverList)
            {
                string email    = approverObj.Approver.Email;
                string subject  = "Program Enrollment Withdrawal (" + approverObj.LMSProgram.LongName + ")"; 
                string message  = student.FullName + " has withdrawn his/her enrollment in " + approverObj.LMSProgram.LongName;
                await _emailSender.SendEmailAsync(email, subject, message);
            }
            ////////////////////////////////////////////////////////////////////////////////////
            // Send email notification to LMS Program's common inbox (if any)
            ////////////////////////////////////////////////////////////////////////////////////
            LMSProgram lmsProgram = await _context.LMSPrograms
                                    .Where(p => p.LMSProgramID == lvProgramEnrollment.LMSProgramID && p.CommonInbox != null)
                                    .AsNoTracking()
                                    .SingleOrDefaultAsync();
            if ( lmsProgram != null
                    && String.IsNullOrEmpty(lmsProgram.CommonInbox) == false )
            {
                string email = lmsProgram.CommonInbox;
                string subject  = "Program Enrollment Withdrawal (" + lmsProgram.LongName + ")";
                string message  = student.FullName + " has withdrawn his/her enrollment in " + lmsProgram.LongName;
                await _emailSender.SendEmailAsync(email, subject, message);
            }              

            /////////////////////////////////////////////////////////////////
            // Redirect to Student Index Page
            /////////////////////////////////////////////////////////////////
            return RedirectToPage("./Index");
        }
        private Task<ApplicationUser> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);

        public override void OnPageHandlerSelected(PageHandlerSelectedContext filterContext)
        {
            string tagName = "[Enrollments][WithdrawModel][OnPageHandlerSelected]";

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
