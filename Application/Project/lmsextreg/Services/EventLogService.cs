using System;
using System.Text;
using Microsoft.Extensions.Logging;
using lmsextreg.Data;
using lmsextreg.Models;
using lmsextreg.ApiModels;
using lmsextreg.Repositories;
using Serilog.Events;

namespace lmsextreg.Services
{
    public class EventLogService : IEventLogService
    {
        private readonly IEventLogRepository _eventLogRepository;
        private readonly IProgramEnrollmentRepository _programEnrollmentRepository;
        private readonly ILogger<EventLogService> _logger; 

        public EventLogService
        (
            IEventLogRepository eventLogRepo,
            IProgramEnrollmentRepository programEnrollmentRepo,
            ILogger<EventLogService> logger
        )
        {
            _eventLogRepository             = eventLogRepo;
            _programEnrollmentRepository    = programEnrollmentRepo;
            _logger = logger;
        }

        public void LogEvent(string eventTypeCode, ApplicationUser appUser)
        {
            var eventLog = new EventLog
            {
                EventTypeCode   = eventTypeCode,
                UserCreatedID   = appUser.Id,
                UserCreatedName = appUser.UserName,
                DataValues = "User=" + appUser.ToString(),
                DateTimeCreated = DateTime.Now
            };

            _eventLogRepository.Add(eventLog);
        }

        public void LogEvent(string eventTypeCode, ApplicationUser appUser, ProgramEnrollment enrollment)
        {
            Console.WriteLine("");
            Console.WriteLine("------------------------------------------------------------------------------------------");
            Console.WriteLine("[EventLogService][LogEvent][ProgramEnrollment] - (ApplicationUser.ToString()):\n"
                                + appUser);
            Console.WriteLine("");
            Console.WriteLine("[EventLogService][LogEvent][ProgramEnrollment] - (ApplicationUser.ToEventLog()):\n" 
                                + appUser.ToEventLog());
            Console.WriteLine("------------------------------------------------------------------------------------------");
            Console.WriteLine("");

            Console.WriteLine("");
            Console.WriteLine("------------------------------------------------------------------------------------------");
            Console.WriteLine("[EventLogService][LogEvent][ProgramEnrollment] - (ProgramEnrollment.ToString()):\n" 
                                + enrollment);
            Console.WriteLine("");
            Console.WriteLine("[EventLogService][LogEvent][ProgramEnrollment] - (ProgramEnrollment.ToEventLog()):\n" 
                                + enrollment.ToEventLog());
            Console.WriteLine("------------------------------------------------------------------------------------------");
            Console.WriteLine("");

            var sb = new StringBuilder();
            sb.Append("User=");
            sb.Append(appUser.ToString());
            //sb.Append("; ");
            //sb.Append(enrollment.ToString());

            var eventLog = new EventLog
            {
                EventTypeCode   = eventTypeCode,
                UserCreatedID   = appUser.Id,
                UserCreatedName = appUser.UserName,
                DataValues = sb.ToString(),
                DateTimeCreated = DateTime.Now
            };

            _eventLogRepository.Add(eventLog);
        } 

        public void LogEvent(string eventTypeCode, ApplicationUser appUser, int programEnrollmentID)
        {
            Console.WriteLine("");
            Console.WriteLine("------------------------------------------------------------------------------------------");
            Console.WriteLine("[EventLogService][LogEvent][int programEnrollmentID] - (ApplicationUser.ToString()):\n"
                                + appUser);
            Console.WriteLine("");
            Console.WriteLine("[EventLogService][LogEvent][int programEnrollmentID] - (ApplicationUser.ToEventLog()):\n" 
                                + appUser.ToEventLog());
            Console.WriteLine("------------------------------------------------------------------------------------------");
            Console.WriteLine("");
            
            Console.WriteLine("Calling _programEnrollmentRepository.Retrieve(programEnrollmentID): ");
            var programEnrollment = _programEnrollmentRepository.Retrieve(programEnrollmentID);
            Console.WriteLine("Returning from _programEnrollmentRepository.Retrieve(programEnrollmentID): ");

            Console.WriteLine("");
            Console.WriteLine("------------------------------------------------------------------------------------------");
            Console.WriteLine("[EventLogService][LogEvent][int programEnrollmentID] - (ProgramEnrollment.ToString()):\n" 
                                + programEnrollment);
            Console.WriteLine("");
            Console.WriteLine("[EventLogService][LogEvent][int programEnrollmentID] - (ProgramEnrollment.ToEventLog()):\n" 
                                + programEnrollment.ToEventLog());
            Console.WriteLine("------------------------------------------------------------------------------------------");
            Console.WriteLine("");

            var sb = new StringBuilder();
            sb.Append("User=");
            sb.Append(appUser);
            // sb.Append("; ");
            // sb.Append(programEnrollment.ToEventLog());

            var eventLog = new EventLog
            {
                EventTypeCode   = eventTypeCode,
                UserCreatedID   = appUser.Id,
                UserCreatedName = appUser.UserName,
                DataValues = sb.ToString(),
                DateTimeCreated = DateTime.Now
            };

            _eventLogRepository.Add(eventLog);
        }
        
        public void LogEvent(UserAdminEvent userAdminEvent)
        {
            EventLog eventLog = new EventLog();

            eventLog.EventTypeCode = userAdminEvent.EventTypeCode;
            eventLog.UserCreatedID = userAdminEvent.AdminCreatedId;
            eventLog.UserCreatedName = userAdminEvent.AdminCreatedEmail;
            eventLog.DataValues = userAdminEvent.DataValues;
            eventLog.DateTimeCreated = DateTime.Now;

            _eventLogRepository.Add(eventLog);
        }
    } 
}