using System;
using System.Collections.Generic;
using System.Linq;
using lmsextreg.Repositories;
using lmsextreg.Data;

namespace lmsextreg.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepo)
        {
            _userRepository = userRepo;
        }

        public ApplicationUser RetrieveUserByEmailAddress(string emailAddress)
        {
            return _userRepository.RetrieveUserByNormalizedEmail(emailAddress.ToUpper());
        }

        public ApplicationUser RetrieveUserByUserId(string userId)
        {
            return _userRepository.RetrieveUserByUserId(userId);
        }

        public List<ApplicationUser> RetrieveAllUsers()
        {
            return _userRepository.RetrieveAllUsers().ToList();
        }

        public int UnlockUser(string userId)
        {
            ApplicationUser appUser = this.RetrieveUserByUserId(userId);
            appUser.LockoutEnd = null;
            return _userRepository.UpdateUser(appUser);
        }
        public int ConfirmEmailAddress(string userId)
        {
            ApplicationUser appUser = this.RetrieveUserByUserId(userId);
            appUser.EmailConfirmed = true;
            return _userRepository.UpdateUser(appUser);
        }
        public int DisableTwoFactorAuth(string userId)
        {
            ApplicationUser appUser = this.RetrieveUserByUserId(userId);
            appUser.TwoFactorEnabled = false;
            return _userRepository.UpdateUser(appUser);
        }
    }
}
