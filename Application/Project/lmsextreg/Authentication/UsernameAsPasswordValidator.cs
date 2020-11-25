using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace lmsextreg.Authentication
{
    public class UsernameAsPasswordValidator<TUser> : IPasswordValidator<TUser>  
        where TUser : IdentityUser
    {
        public Task<IdentityResult> ValidateAsync(UserManager<TUser> manager, TUser user, string password)
        {
            if (string.Equals(user.UserName, password, StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine("[UsernameAsPasswordValidator][ValidateAsync] - (Password Validation FAILED)");

                return Task.FromResult(IdentityResult.Failed(new IdentityError
                {
                    Code = "UsernameAsPassword",
                    Description = "You cannot use your username as your password"
                }));
            }
            
            Console.WriteLine("[UsernameAsPasswordValidator][ValidateAsync] - (Password Validation SUCCEEDED)");            
            return Task.FromResult(IdentityResult.Success);
        }
    }    
}