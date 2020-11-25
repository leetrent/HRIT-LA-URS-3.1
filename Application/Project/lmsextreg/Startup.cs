using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Hosting;
using lmsextreg.Data;
using lmsextreg.Services;
using lmsextreg.Authorization;
using lmsextreg.Authentication;
using lmsextreg.Repositories;
using lmsextreg.Constants;

namespace lmsextreg
{
    public class Startup
    {
        // ILogger _logger = null;

        public Startup(IConfiguration configuration, ILoggerFactory loggerFactory)
        {
            Configuration = configuration;
            //  _logger = loggerFactory.CreateLogger<GlobalFiltersLogger>();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            ////////////////////////////////////////////////////////////////////////////////////////////////////
            // CLOUD.GOV (please do not remove)
            // String connectionString = buildConnectionString();
            ////////////////////////////////////////////////////////////////////////////////////////////////////            
             
             ////////////////////////////////////////////////////////////////////////////////////////////////////   
             // Database Connection Parameters
             ////////////////////////////////////////////////////////////////////////////////////////////////////   
            String connectionString = Configuration.GetValue<string>("DatabaseConnection");
            
            // WRITE CONNECTION STRING TO THE CONSOLE
            // Console.WriteLine("********************************************************************************");
            // Console.WriteLine("[Startup] Connection String: " + connectionString);
            // Console.WriteLine("********************************************************************************");

            // NOW THAT WE HAVE OUR CONNECTION STRING, WE CAN ESTABLISH OUR DB CONTEXT
            //services.AddDbContext<ApplicationDbContext>
            //(
            //    options => options.UseMySQL(connectionString)
            //);

            services.AddEntityFrameworkMySql();
            services.AddDbContext<ApplicationDbContext>(
                options => options.UseMySql(connectionString),  ServiceLifetime.Scoped);

            services.AddIdentity<ApplicationUser, IdentityRole>(config =>
            {
                config.SignIn.RequireConfirmedEmail = true;
            })
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders()
                .AddPasswordValidator<UsernameAsPasswordValidator<ApplicationUser>>();

            services.Configure<IdentityOptions>(options =>
            {
                // Password settings
                options.Password.RequiredLength = 12;
                options.Password.RequireDigit = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = true;
                options.Password.RequiredUniqueChars = 6;

                // Lockout settings
                options.Lockout.MaxFailedAccessAttempts = 3;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
                options.Lockout.AllowedForNewUsers = true;

                // User settings
                options.User.RequireUniqueEmail = true; 
            });
                

            /***************************************************************************************
                
                Require authenticated users
                
                Set the default authentication policy to require users to be authenticated.
                You can opt out of authentication at the Razor Page, controller
                or action method level with the [AllowAnonymous] attribute. 
                
                Setting the default authentication policy to require users to be authenticated
                protects newly added Razor Pages and controllers. 
                
                Having authentication required by default is safer than relying on new controllers
                and Razor Pages to include the [Authorize] attribute.              
            ***************************************************************************************/
            services.AddMvc();
            // requires: using Microsoft.AspNetCore.Authorization;
            //           using Microsoft.AspNetCore.Mvc.Authorization;
            services.AddMvc(config =>
            {
                var policy = new AuthorizationPolicyBuilder()
                                .RequireAuthenticatedUser()
                                .Build();
                config.Filters.Add(new AuthorizeFilter(policy));
            });            
            /***************************************************************** 
                With the requirement of all users authenticated, 
                the AuthorizeFolder and AuthorizePage calls are not required.
            ******************************************************************/
            // services.AddMvc()
            //     .AddRazorPagesOptions(options =>
            //     {
            //         options.Conventions.AuthorizeFolder("/Account/Manage");
            //         options.Conventions.AuthorizePage("/Account/Logout");
            //     });
            /*******************************************************************/
            
            /***************************************************************** 
                Register no-op EmailSender used by account confirmation and password
                reset during development
                For more information on how to enable account confirmation and password reset,
                please visit https://go.microsoft.com/fwlink/?LinkID=532713
            ******************************************************************/
            services.AddSingleton<IEmailSender, EmailSender>();

            // Configure startup to use AuthMessageSenderOptions
            services.Configure<AuthMessageSenderOptions>(Configuration);

            // Register the authorization handlers
            // (not being used at this time)
            // services.AddScoped<IAuthorizationHandler, StudentAuthorizationHandler>();
            // services.AddScoped<IAuthorizationHandler, ApproverAuthorizationHandler>();

            services.AddAuthorization(options =>
            {
                options.AddPolicy("CanAccessStudentLink", policy =>
                    policy.Requirements.Add(new CanAccessStudentLink()));

                options.AddPolicy("CanAccessApproverLink", policy =>
                    policy.Requirements.Add(new CanAccessApproverLink()));    

                options.AddPolicy("CanAccessProfileLink", policy =>
                    policy.Requirements.Add(new CanAccessProfileLink()));

                options.AddPolicy("CanAccessAdminLink", policy =>
                    policy.Requirements.Add(new CanAccessAdminLink()));
            });

            services.AddScoped<IAuthorizationHandler, CanAccessStudentLinkHandler>();
            services.AddScoped<IAuthorizationHandler, CanAccessApproverLinkHandler>();
            services.AddScoped<IAuthorizationHandler, CanAccessProfileLinkHandler>();
            services.AddScoped<IAuthorizationHandler, CanAccessAdminLinkHandler>();

            // Register EventLogRepository
            services.AddScoped<IEventLogRepository, EventLogRepository>();    

            // Register ProgramEnrollmentRepository
            services.AddScoped<IProgramEnrollmentRepository, ProgramEnrollmentRepository>();    

            // Register SessionCookietRepository
            services.AddScoped<ISessionCookieRepository, SessionCookieRepository>();  

            // Register EmailTokenRepository
            services.AddScoped<IEmailTokenRepository, EmailTokenRepository>();

            // Register UserRepository
            services.AddScoped<IUserRepository, UserRepository>();

            // Register EventLogService
            services.AddScoped<IEventLogService, EventLogService>(); 

            // Register ReCaptcha Service (Google)
            services.AddScoped<IReCaptchaService, ReCaptchaService>();    

            // Register Session Cookie Service
            services.AddScoped<ISessionCookieService, SessionCookieService>();  

            // Register ConfirmEmailService
            services.AddScoped<IConfirmEmailService, ConfirmEmailService>();

            // Register UserService
            services.AddScoped<IUserService, UserService>();


            // Configure Application Cookie
            // services.ConfigureApplicationCookie(options =>
            // {
            //     options.Cookie.HttpOnly = true;
            //     // options.Cookie.Expiration = TimeSpan.FromSeconds(60);
            //     options.Cookie.Name = "GSALearningAcademy";
            //     options.LoginPath = "/Account/Login";
            //     options.LogoutPath = "/Account/Logout";
            //     options.AccessDeniedPath = "/Account/AccessDenied";
            //     options.SlidingExpiration = true;
            //     options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
            // });

            services.ConfigureApplicationCookie(options =>
            {
                // options.Cookie.Expiration = TimeSpan.FromSeconds(60);
                options.Cookie.Name = MiscConstants.SESSION_COOKIE_NAME;
                options.Cookie.HttpOnly = true;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                options.Cookie.SameSite = SameSiteMode.Strict;
                options.LoginPath = "/Account/Login";
                options.LogoutPath = "/Account/Logout";
                options.AccessDeniedPath = "/Account/AccessDenied";
                options.SlidingExpiration = true;
                options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
            });

            // services.AddMvc(options =>
            // {
            //     options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
            // });

            // services.AddMvc()
            //    .AddRazorPagesOptions(options =>
            //    {
            //        options.Conventions.AddFolderApplicationModelConvention(
            //            "/Enrollments",
            //            model => model.Filters.Add(new ProtectedPageFilter(_logger)));
            //    });

            // services.AddMvc()
            //    .AddRazorPagesOptions(options =>
            //    {
            //        options.Conventions.AddFolderApplicationModelConvention(
            //            "/Enrollments",
            //            model => model.Filters.Add(new ProtectedPageFilter(_logger)));
            //        options.Conventions.AddFolderApplicationModelConvention(
            //            "/Approvals",
            //            model => model.Filters.Add(new ProtectedPageFilter(_logger)));
            //    });

            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromSeconds(30 * 60);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            app.UsePathBase("/lms31");

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseStaticFiles();

            app.UseSession();

            //app.UseMvc();
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
            });


            ////////////////////////////////////////////////////////////////////////////////////////////////////
            // LOG FILE SETUP
            //  1. see EnvironmentVariables.txt
            //  2. see lmsextreg.Constants.APPSETTINGS_FILE_NAME            
            ////////////////////////////////////////////////////////////////////////////////////////////////////
            string logFileDirectory = Environment.GetEnvironmentVariable("LOGFILE_DIRECTORY");
            Console.WriteLine("********************************************************************************");
            Console.WriteLine("[Startup] logFileDirectory: '" + logFileDirectory + "'");
            Console.WriteLine("********************************************************************************");

            if ( String.IsNullOrEmpty(logFileDirectory) == false)
            {
                loggerFactory.AddFile(logFileDirectory + "/" + MiscConstants.APP_NAME + "-{Date}.log");
            }
        }

        private String buildConnectionString()
        {
             String connectionString = null;
            try
            {
                connectionString = Environment.GetEnvironmentVariable("LOCAL_CONNECTION_STRING");
                if (connectionString == null)
                {
                    string vcapServices = System.Environment.GetEnvironmentVariable("VCAP_SERVICES");
                    if (vcapServices != null)
                    {
                        dynamic json = JsonConvert.DeserializeObject(vcapServices);
                        foreach (dynamic obj in json.Children())
                        {
                            dynamic credentials = (((JProperty)obj).Value[0] as dynamic).credentials;
                            if (credentials != null)
                            {
                                string host     = credentials.host;
                                string username = credentials.username;
                                string password = credentials.password;
                                string port     = credentials.port;
                                string db_name  = credentials.db_name;

                                connectionString = "Username=" + username + ";"
                                    + "Password=" + password + ";"
                                    + "Host=" + host + ";"
                                    + "Port=" + port + ";"
                                    + "Database=" + db_name + ";Pooling=true;";
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception in [Startup.buildConnectionString()]:");
                Console.WriteLine(e);
            }
             return connectionString;
        }
    }
}
