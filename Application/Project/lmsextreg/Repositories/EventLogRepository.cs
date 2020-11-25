
using lmsextreg.Data;
using lmsextreg.Models;

namespace lmsextreg.Repositories
{
    public class EventLogRepository : IEventLogRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public EventLogRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void Add(EventLog eventLog)
        {
            _dbContext.EventLogs.Add(eventLog);
            _dbContext.SaveChanges();
        }
    }
}