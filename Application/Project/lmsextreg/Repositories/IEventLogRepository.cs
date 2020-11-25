using lmsextreg.Models;

namespace lmsextreg.Repositories
{
    public interface IEventLogRepository
    {
        void Add(EventLog eventLog);
    }
}