using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using lmsextreg.Data;
using lmsextreg.Models;
using lmsextreg.Services;

namespace lmsextreg.Pages.Enrollments
{    
    [Authorize(Roles = "STUDENT")]
    public class DetailsModel : PageModel
    {
        private readonly lmsextreg.Data.ApplicationDbContext _dbContext;
        private readonly UserManager<ApplicationUser> _userManager; 
        private readonly ISessionCookieService _sessionCookieService;
        private readonly ILogger<DetailsModel> _logger;         

        public DetailsModel (   lmsextreg.Data.ApplicationDbContext dbContext, 
                                UserManager<ApplicationUser> userMgr,
                                ISessionCookieService sessionCookieSvc,
                                ILogger<DetailsModel> logger                                
                            )
        {
            _dbContext = dbContext;
            _userManager = userMgr;
            _sessionCookieService = sessionCookieSvc;
            _logger = logger;             
        }

        public ProgramEnrollment ProgramEnrollment { get; set; }
        public IList<EnrollmentHistory> EnrollmentHistory { get; set; }

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
                                .Include(p => p.LMSProgram)
                                .SingleOrDefaultAsync(m => m.ProgramEnrollmentID == id);

            ////////////////////////////////////////////////////////////
            // Return "Not Found" if record doesn't exist
            ////////////////////////////////////////////////////////////
            if (ProgramEnrollment == null)
            {
                return NotFound();
            } 

            ////////////////////////////////////////////////////////////////////
            // Step #2:
            // Now that record exists, make sure that the logged-in user
            // is authorized to view the deatails of  this program enrollment
            ///////////////////////////////////////////////////////////////////
            var loggedInUserID = _userManager.GetUserId(User);
            ProgramEnrollment = null;
            ProgramEnrollment = await _dbContext.ProgramEnrollments
                                .Where(pe => pe.StudentUserId == loggedInUserID && pe.ProgramEnrollmentID == id) 
                                .Include(p => p.LMSProgram)
                                .Include(pe => pe.Student)
                                    .ThenInclude(s => s.SubAgency)
                                    .ThenInclude(sa => sa.Agency)
                                .Include(pe => pe.EnrollmentStatus)                                
                                .SingleOrDefaultAsync();                   
                  
            /////////////////////////////////////////////////////////////
            // We already know that record exists from Step #1 so if we
            // get a "Not Found" in Step #2, we know it's because the 
            // logged-in user is not authorized to view the details of
            // this program enrollment.
            /////////////////////////////////////////////////////////////
            if (ProgramEnrollment == null)
            {
                return Unauthorized();
            }

            ////////////////////////////////////////////////////////////
            // Retrieve enrolloment history for this enrollment record.
            ////////////////////////////////////////////////////////////
            EnrollmentHistory = await _dbContext.EnrollmentHistories
                                    .Where(eh => eh.ProgramEnrollmentID == ProgramEnrollment.ProgramEnrollmentID)
                                    .Include(eh => eh.Actor)
                                    .Include(eh => eh.StatusTransition)
                                    .OrderBy(eh => eh.EnrollmentHistoryID)
                                    .ToListAsync();            

            //////////////////////////////////////////////////////////////////////////////////
            // If we get this far, then record was found and user is authorized to access it
            ////////////////////////////////////////////////////////////////////////////////// 
            return Page();
        }

        public override void OnPageHandlerSelected(PageHandlerSelectedContext filterContext)
        {
            string tagName = "[Enrollments][DetailsModel][OnPageHandlerSelected]";

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
