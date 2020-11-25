using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using lmsextreg.Models;
using lmsextreg.Data;
using lmsextreg.Repositories;

namespace lmsextreg.Services
{
    public class ConfirmEmailService : IConfirmEmailService
    {
        public readonly ApplicationDbContext _dbContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailTokenRepository _emailTokenRepo;

        public ConfirmEmailService
        (
            ApplicationDbContext dbCtxt, 
            UserManager<ApplicationUser> userMgr, 
            IEmailTokenRepository repo)
        {
            _dbContext = dbCtxt;
            _userManager = userMgr;
            _emailTokenRepo = repo;
        }


        public bool IsConfirmed(ApplicationUser appUser, string tokenValue)
        {
            Console.WriteLine("["   + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + "][ConfirmEmailService][IsConfirmed] => ApplicationUser.Id: " 
                                    + (appUser.Id));
            Console.WriteLine("["   + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + "][ConfirmEmailService][IsConfirmed] => ApplicationUser.Email: " 
                                    + (appUser.Email));
            Console.WriteLine("["   + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + "][ConfirmEmailService][IsConfirmed] => tokenValue: '" 
                                    + tokenValue
                                    + "'");
                            
            ///////////////////////////////////////////////////////////////////////////
            // Attempt to validate the email verification token using the new method.
            //////////////////////////////////////////////////////////////////////////
            Console.WriteLine("["   + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") 
                                    + "][ConfirmEmailService][IsConfirmed] => Calling _emailTokenRepo.Retrieve() ...");
            
            EmailToken emailToken = _emailTokenRepo.Retrieve(appUser.Id);
            
            Console.WriteLine("[" + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") 
                                + "][ConfirmEmailService][IsConfirmed] => EmailToken IS NULL: " 
                                + (emailToken == null));

            if ( emailToken == null )
            {
                //////////////////////////////////////////////////////////////////////////
                // EmailToken using the new method WAS NOT FOUND.
                // Attempt to validate email verification token 
                // using the OLD method.
                //////////////////////////////////////////////////////////////////////////
                return this.isConfirmedUsingOldMethod(appUser, tokenValue);
            }
            else
            {
                //////////////////////////////////////////////////////////////////////////
                // EmailToken using the new method WAS FOUND.
                // Attempt to validate email verification token 
                // using the NEW method.
                //////////////////////////////////////////////////////////////////////////
                return this.isConfirmedUsingNewMethod(emailToken, appUser, tokenValue);
            }
        }    

        ////////////////////////////////////////////////////////////////////////////////
        // New method of validating email verification token
        ////////////////////////////////////////////////////////////////////////////////
        private bool isConfirmedUsingNewMethod(EmailToken emailToken, ApplicationUser appUser, string tokenValue)
        {
            Console.WriteLine("["   + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + "][ConfirmEmailService][isConfirmedUsingNewMethod] => BEGIN tokenValue: " );
            Console.WriteLine("'"   + tokenValue + "'");
            Console.WriteLine("["   + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + "][ConfirmEmailService][isConfirmedUsingNewMethod] => :END tokenValue" );
            
            Console.WriteLine("");
            
            Console.WriteLine("["   + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + "][ConfirmEmailService][isConfirmedUsingNewMethod] => BEGIN EmailToken.TokenValue: " );
            Console.WriteLine("'"   + emailToken.TokenValue + "'");
            Console.WriteLine("["   + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + "][ConfirmEmailService][isConfirmedUsingNewMethod] => :END EmailToken.TokenValue" );

            if ( emailToken.TokenValue.Equals(tokenValue))
            {
                Console.WriteLine("["   + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") 
                                        + "][ConfirmEmailService][isConfirmedUsingNewMethod] => Email tokens ARE EQUAL - setting EmailConfirmed column to TRUE in AspNetUsers table." );

                appUser.EmailConfirmed = true;
                _dbContext.ApplicationUsers.Update(appUser);
                _dbContext.SaveChanges();

                Console.WriteLine("["   + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") 
                                        + "][ConfirmEmailService][isConfirmedUsingNewMethod] => Email tokens ARE EQUAL - returning TRUE" );
                return true;
            }

            Console.WriteLine("["   + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") 
                                    + "][ConfirmEmailService][isConfirmedUsingNewMethod] => Email tokens ARE NOT EQUAL - returning FALSE" );

            return false;          
        }

        private bool isConfirmedUsingOldMethod(ApplicationUser appUser, string tokenValue)
        {
            Console.WriteLine("["   + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") 
                                    + "][ConfirmEmailService][isConfirmedUsingOldMethod] => Caling _userManager.ConfirmEmailAsync() ...");

            var identityResult =  _userManager.ConfirmEmailAsync(appUser, tokenValue).Result;
            
            Console.WriteLine("[" + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + "][ConfirmEmailService][isConfirmedUsingOldMethod] => IdentityResult.ToString(): " 
                                + (identityResult.ToString()));
            Console.WriteLine("[" + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + "][ConfirmEmailService][isConfirmedUsingOldMethod] => IdentityResult.Succeeded: " 
                                + (identityResult.Succeeded));

            if ( identityResult.Succeeded )
            {
                Console.WriteLine("["   + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") 
                                        + "][ConfirmEmailService][isConfirmedUsingOldMethod] => (identityResult.Succeeded == TRUE) - returning TRUE" );
                return true;
            }
            else
            {
                Console.WriteLine("["   + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") 
                                        + "][ConfirmEmailService][isConfirmedUsingOldMethod] => (identityResult.Succeeded == FALSE) - returning FALSE" );
                this.printErrors(identityResult.Errors);
                return false;
            }
        }

        private void printErrors(IEnumerable<IdentityError> identityErrors)
        {
                foreach ( var error in identityErrors)
                {
                    Console.WriteLine("");
                    Console.WriteLine("[" + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + "][ConfirmEmailService][printErrors] => IdentityError.code: " 
                        + (error.Code) + " / IdentityError.Description: " + error.Description);
                    Console.WriteLine("[" + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + "][ConfirmEmailService][printErrors] => IdentityError.ToString(): " 
                        + error.ToString()); 
                    Console.WriteLine("");
                }            
        }
    }
}