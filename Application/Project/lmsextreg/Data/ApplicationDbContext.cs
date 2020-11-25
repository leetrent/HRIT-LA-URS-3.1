using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using lmsextreg.Models;
using lmsextreg.Constants;

namespace lmsextreg.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Agency> Agencies { get; set; }
        public DbSet<SubAgency> SubAgencies { get; set; }
        public DbSet<LMSProgram> LMSPrograms { get; set; }
        public DbSet<ProgramApprover> ProgramApprovers { get; set; }
        public DbSet<EnrollmentStatus> EnrollmentStatuses { get; set; }
        public DbSet<ProgramEnrollment> ProgramEnrollments { get; set; }
        public DbSet<StatusTransition> StatusTransitions { get; set; }
        public DbSet<EnrollmentHistory> EnrollmentHistories { get; set; }
        public DbSet<EventType> EventTypes { get; set; }
        public DbSet<EventLog> EventLogs { get; set; }  
        public DbSet<SessionCookie> SessionCookies { get; set; } 
        public DbSet<EmailToken> EmailTokens { get; set; } 
        public DbSet<ApplicationUser> ApplicationUsers { get; set; } 

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.HasDefaultSchema(MiscConstants.DB_SCHEMA_NAME);
          
            /***************************************************************************
             Customize the ASP.NET Identity model and override the defaults if needed.
             For example, you can rename the ASP.NET Identity table names and more.
             Add your customizations after calling base.OnModelCreating(builder);
             ***************************************************************************/
            builder.Entity<Agency>().ToTable("Agency");
            builder.Entity<SubAgency>().ToTable("SubAgency");
            builder.Entity<LMSProgram>().ToTable("LMSProgram");
            builder.Entity<ProgramApprover>().ToTable("ProgramApprover");
            builder.Entity<EnrollmentStatus>().ToTable("EnrollmentStatus");
            builder.Entity<ProgramEnrollment>().ToTable("ProgramEnrollment");
            builder.Entity<StatusTransition>().ToTable("StatusTransition");
            builder.Entity<EnrollmentHistory>().ToTable("EnrollmentHistory");
            builder.Entity<EventType>().ToTable("EventType");
            builder.Entity<EventLog>().ToTable("EventLog"); 
            builder.Entity<SessionCookie>().ToTable("SessionCookie");  
            builder.Entity<EmailToken>().ToTable("EmailToken");            

            /************************************************************************
             There are some configurations that can only be done with the fluent API
             (specifying a composite PK).
             ************************************************************************/            
            
            /////////////////////////////////////////////////////////////////////////
            //ProgramApprover: 
            // - Composite Primary Key
            /////////////////////////////////////////////////////////////////////////
            builder.Entity<ProgramApprover>()
                .HasKey( pa => new { pa.LMSProgramID, pa.ApproverUserId } );      

           /////////////////////////////////////////////////////////////////////////
            // ProgramApprover:
            //  - Foreign Key (ApplicationUser.Id)
            /////////////////////////////////////////////////////////////////////////
            builder.Entity<ProgramApprover>()
                .HasOne( pa => pa.Approver)
                .WithMany()
                .HasForeignKey(pa => pa.ApproverUserId);                                                     
            
            /////////////////////////////////////////////////////////////////////////                           
            // ProgramEnrollment:
            // - Unique Key Constraint Combination (LMSProgramID, LearnerUserId)
            /////////////////////////////////////////////////////////////////////////
            builder.Entity<ProgramEnrollment>()
                .HasIndex( p => new {p.LMSProgramID, p.StudentUserId} )
                .IsUnique();

            /////////////////////////////////////////////////////////////////////////
            // ProgramEnrollment:
            //  - Foreign Key (EnrollmentStatus.StatusCode)
            /////////////////////////////////////////////////////////////////////////
            builder.Entity<ProgramEnrollment>()
                .HasOne( pe => pe.EnrollmentStatus)
                .WithMany()
                .HasForeignKey(pe => pe.StatusCode);

           /////////////////////////////////////////////////////////////////////////
            // ProgramEnrollment:
            //  - Foreign Key (ApplicationUser.Id -  Student)
            /////////////////////////////////////////////////////////////////////////
            builder.Entity<ProgramEnrollment>()
                .HasOne( pe => pe.Student)
                .WithMany()
                .HasForeignKey(pe => pe.StudentUserId);

           /////////////////////////////////////////////////////////////////////////
            // ProgramEnrollment:
            //  - Foreign Key (ApplicationUser.Id - Approver)
            /////////////////////////////////////////////////////////////////////////
            builder.Entity<ProgramEnrollment>()
                .HasOne( pe => pe.Approver)
                .WithMany()
                .HasForeignKey(pe => pe.ApproverUserId);
                
            /////////////////////////////////////////////////////////////////////////                           
            // EnrollmentStatus:
            // - Unique Key Constraint
            /////////////////////////////////////////////////////////////////////////
            builder.Entity<EnrollmentStatus>()
                .HasIndex(es => es.StatusLabel)
                .IsUnique();                         
            
            /////////////////////////////////////////////////////////////////////////                           
            // StatusTransition:
            // - Unique Key Constraint Combination (FromStatusCode, ToStatusCode)
            /////////////////////////////////////////////////////////////////////////
            builder.Entity<StatusTransition>()
                .HasIndex( st => new {st.FromStatusCode, st.ToStatusCode} )
                .IsUnique();

            /////////////////////////////////////////////////////////////////////////
            // StatusTransition:
            //  - Foreign Key (EnrollmentStatus.StatusCode)
            /////////////////////////////////////////////////////////////////////////
            builder.Entity<StatusTransition>()
                .HasOne( st => st.FromStatus)
                .WithMany()
                .HasForeignKey(st => st.FromStatusCode);

           /////////////////////////////////////////////////////////////////////////
            // StatusTransition:
            //  - Foreign Key (EnrollmentStatus.StatusCode)
            /////////////////////////////////////////////////////////////////////////
            builder.Entity<StatusTransition>()
                .HasOne( st => st.ToStatus)
                .WithMany()
                .HasForeignKey(st => st.ToStatusCode);     

            /////////////////////////////////////////////////////////////////////////
            // EnrollmentHistory:
            //  - Foreign Key (ProgramEnrollment.ProgramEnrollmentID)
            /////////////////////////////////////////////////////////////////////////
            // builder.Entity<EnrollmentHistory>()
            //     .HasOne( eh => eh.ProgramEnrollment)
            //     .WithMany()
            //     .HasForeignKey(eh => eh.ProgramEnrollmentID);      

            /////////////////////////////////////////////////////////////////////////
            // EnrollmentHistory:
            //  - Foreign Key (StatusTransition.StatusTransitionID)
            /////////////////////////////////////////////////////////////////////////
            builder.Entity<EnrollmentHistory>()
                .HasOne( eh => eh.StatusTransition)
                .WithMany()
                .HasForeignKey(eh => eh.StatusTransitionID);   

            /////////////////////////////////////////////////////////////////////////
            // EnrollmentHistory:
            //  - Foreign Key (StatusTransition.StatusTransitionID)
            /////////////////////////////////////////////////////////////////////////
            builder.Entity<EnrollmentHistory>()
                .HasOne( eh => eh.Actor)
                .WithMany()
                .HasForeignKey(eh => eh.ActorUserId);

            /////////////////////////////////////////////////////////////////////////                           
            // EventType:
            // - Unique Key Constraint (EventTypeCode)
            /////////////////////////////////////////////////////////////////////////
            // builder.Entity<EventType>()
            //     .HasIndex(et => et.EventTypeCode)
            //     .IsUnique();      
    
            /////////////////////////////////////////////////////////////////////////                           
            // EventType:
            // - Unique Key Constraint (EventTypeLabel)
            /////////////////////////////////////////////////////////////////////////
            builder.Entity<EventType>()
                .HasIndex(et => et.EventTypeLabel)
                .IsUnique();  

            /////////////////////////////////////////////////////////////////////////
            // EventLog:
            //  - Foreign Key (EventLog.EventTypeID => EventType.EventTypeID)
            /////////////////////////////////////////////////////////////////////////
            builder.Entity<EventLog>()
                .HasOne( el => el.EventType)
                .WithMany()
                .HasForeignKey(el => el.EventTypeCode);                 

            /////////////////////////////////////////////////////////////////////////
            // EventLog:
            //  - Foreign Key (EventLog.UserCreatedID  => ApplicationUser.Id)
            /////////////////////////////////////////////////////////////////////////
            builder.Entity<EventLog>()
                .HasOne( el => el.UserCreated)
                .WithMany()
                .HasForeignKey(el => el.UserCreatedID);   

            //////////////////////////////////////////////////////////////////////////////////////////
            // Application User (AspNetUsers table):
            //////////////////////////////////////////////////////////////////////////////////////////
            //  - This is being done to fix the following problem:
            //  - "No coercion operator is defined between types 'System.Int16' and 'System.Boolean'"
            //  - This problem was encountered when attempting to seed the database after
            //  - migrating from PostgreSQL to MySQL
            //////////////////////////////////////////////////////////////////////////////////////////
            builder.Entity<ApplicationUser>(au =>
            {
              au.Property(u => u.EmailConfirmed).HasColumnType("tinyint(1)");
              au.Property(u => u.PhoneNumberConfirmed).HasColumnType("tinyint(1)");
              au.Property(u => u.TwoFactorEnabled).HasColumnType("tinyint(1)");
              au.Property(u => u.LockoutEnabled).HasColumnType("tinyint(1)");
              au.Property(u => u.RulesOfBehaviorAgreedTo).HasColumnType("tinyint(1)");
            });

            //////////////////////////////////////////////////////////////////////////////////////////
            // ProgramApprover (ProgramApprover table):
            //////////////////////////////////////////////////////////////////////////////////////////
            //  - This is being done to fix the following problem:
            //  - "No coercion operator is defined between types 'System.Int16' and 'System.Boolean'"
            //  - This problem was encountered when attempting to seed the database after
            //  - migrating from PostgreSQL to MySQL
            //////////////////////////////////////////////////////////////////////////////////////////
            builder.Entity<ProgramApprover>(pa =>
            {
              pa.Property(a => a.EmailNotify).HasColumnType("tinyint(1)");
            });      

            //////////////////////////////////////////////////////////////////////////////////////////
            // SessionCookie:
            //   -- Composite Primery Key (UserName, CookieName) 
            //   -- Using Fluent API to specify composite primary key
            //////////////////////////////////////////////////////////////////////////////////////////
            builder.Entity<SessionCookie>()
                .HasKey(sc => new { sc.UserName, sc.CookieName });

            //////////////////////////////////////////////////////////////////////////////////////////
            // EmailToken:
            //  -- Primery Key (UserId) 
            //////////////////////////////////////////////////////////////////////////////////////////
            builder.Entity<EmailToken>()
                .HasKey( et => new { et.UserId } );

            /////////////////////////////////////////////////////////////////////////
            // EmailToken:
            //  -- Foreign Key (EmailToken.UserId  => ApplicationUser.Id)
            /////////////////////////////////////////////////////////////////////////
            builder.Entity<EmailToken>()
                .HasOne( et => et.User)
                .WithMany()
                .HasForeignKey(et => et.UserId); 
        }
    }
}