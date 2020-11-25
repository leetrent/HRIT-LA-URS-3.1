using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc.Filters;
using System.ComponentModel.DataAnnotations;
using lmsextreg.Data;
using lmsextreg.Models;
using lmsextreg.Constants;
using lmsextreg.Services;

namespace lmsextreg.Pages.Enrollments
{
    [Authorize(Roles = "STUDENT")]
    public class ReenrollModel : PageModel
    {
        private readonly lmsextreg.Data.ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _emailSender;
        private readonly IEventLogService _eventLogService;
        private readonly ISessionCookieService _sessionCookieService;
        private readonly ILogger<ReenrollModel> _logger;         

        public ReenrollModel
        (
                lmsextreg.Data.ApplicationDbContext context, 
                UserManager<ApplicationUser> userMgr,
                IEmailSender emailSender,
                IEventLogService eventLogSvc,
                ISessionCookieService sessionCookieSvc,
                ILogger<ReenrollModel> logger
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
            // STATUS TRANSITION: WITHDRAWN TO PENDING
            //////////////////////////////////////////////////////////////////////
            if (lvProgramEnrollment.StatusCode.Equals(StatusCodeConstants.WITHDRAWN))
            {
                lvStatusTranstion = await _context.StatusTransitions
                                    .Where(st => st.TransitionCode == TransitionCodeConstants.WITHDRAWN_TO_PENDING)
                                    .SingleOrDefaultAsync();
            }
            //////////////////////////////////////////////////////////////////////            
            // STATUS TRANSITION: REVOKED TO PENDING
            //////////////////////////////////////////////////////////////////////
            if (lvProgramEnrollment.StatusCode.Equals(StatusCodeConstants.REVOKED))
            {
                lvStatusTranstion = await _context.StatusTransitions
                                    .Where(st => st.TransitionCode == TransitionCodeConstants.REVOKED_TO_PENDING)
                                    .SingleOrDefaultAsync();
            }
            //////////////////////////////////////////////////////////////////////            
            // STATUS TRANSITION: DENIED TO PENDING
            //////////////////////////////////////////////////////////////////////
            if (lvProgramEnrollment.StatusCode.Equals(StatusCodeConstants.DENIED))
            {
                lvStatusTranstion = await _context.StatusTransitions
                                    .Where(st => st.TransitionCode == TransitionCodeConstants.DENIED_TO_PENDING)
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
            lvProgramEnrollment.StatusCode = StatusCodeConstants.PENDING;
            //lvProgramEnrollment.ApproverUserId = _userManager.GetUserId(User);
            _context.ProgramEnrollments.Update(lvProgramEnrollment);
            await _context.SaveChangesAsync();

            ////////////////////////////////////////////////////////////////////////////////
            // Retrieve 'User as Student' for event logging and email notification purposes
            ////////////////////////////////////////////////////////////////////////////////
            ApplicationUser student = await GetCurrentUserAsync();

            /////////////////////////////////////////////////////////////////////////////////
            // Log the 'REENROLLMENT_REQUSTED' event
            ////////////////////////////////////////////////////////////////////////////////
            // _eventLogService.LogEvent
            // (
            //     EventTypeCodeConstants.REENROLLMENT_REQUSTED, 
            //     student, 
            //     lvProgramEnrollment.ProgramEnrollmentID
            // );    

            _eventLogService.LogEvent
            (
                EventTypeCodeConstants.REENROLLMENT_REQUSTED, 
                student, 
                lvProgramEnrollment
            );  



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
                string subject  = "Program Re-Enrollment Request (" + approverObj.LMSProgram.LongName + ")"; 
                string message  = student.FullName + " has requested to re-enroll in " + approverObj.LMSProgram.LongName;
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
                string subject  = "Program Re-Enrollment Request (" + lmsProgram.LongName + ")";
                string message  = student.FullName + " has requested to re-enroll in " + lmsProgram.LongName;
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
            string tagName = "[Enrollments][ReenrollModel][OnPageHandlerSelected]";

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
