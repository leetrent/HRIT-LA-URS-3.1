
using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using lmsextreg.Data;
using lmsextreg.Models;

namespace lmsextreg.Repositories
{
    public class ProgramEnrollmentRepository : IProgramEnrollmentRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public ProgramEnrollmentRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        //public ProgramEnrollment Retrieve(int programEnrollmentID)
        // {
        //     Console.WriteLine("[ProgramEnrollmentRepository][Retrieve] - (programEnrollmentID): " + programEnrollmentID);
        //     return _dbContext.ProgramEnrollments
        //             .Where(pe => pe.ProgramEnrollmentID == programEnrollmentID) 
        //             .Include(pe => pe.LMSProgram)
        //             .Include(pe => pe.Student)    
        //                 .ThenInclude(s => s.SubAgency)
        //                 .ThenInclude(sa => sa.Agency)
        //             .Include(pe => pe.EnrollmentStatus)                            
        //             .SingleOrDefault(); 
        // }      

        public ProgramEnrollment Retrieve(int programEnrollmentID)
        {
            Console.WriteLine("[ProgramEnrollmentRepository][Retrieve] - (programEnrollmentID): " + programEnrollmentID);
            return _dbContext.ProgramEnrollments
                    .Where(pe => pe.ProgramEnrollmentID == programEnrollmentID) 
                    .Include(pe => pe.LMSProgram)
                    .Include(pe => pe.EnrollmentStatus)                            
                    .Include(pe => pe.Student)    
                        .ThenInclude(stud => stud.SubAgency)
                        .ThenInclude(stud => stud.Agency)
                    .Include(pe => pe.Approver)    
                        .ThenInclude(appr => appr.SubAgency)
                        .ThenInclude(appr => appr.Agency)                        
                    .SingleOrDefault(); 
        }            
    }
}