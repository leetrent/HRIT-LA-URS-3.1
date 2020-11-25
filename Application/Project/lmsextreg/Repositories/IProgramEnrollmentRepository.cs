using lmsextreg.Models;

namespace lmsextreg.Repositories
{
    public interface IProgramEnrollmentRepository
    {
        ProgramEnrollment Retrieve(int programEnrollmentID);
    }
}