using System;

namespace Services.UserServ
{
    public class UserService : IUserService
    {
        public IUserRepository UserRepo { get; }

        public UserService(IUserRepository userRepo)
        {
            UserRepo = userRepo;
        }

        public void Create(IUser user)
        {
            UserRepo.Insert(user);
            UserRepo.Save();
        }

        public IUser? GetByEmail(string email)
        {
            return UserRepo.GetByEmail(email);
        }

        public IUser? GetById(int userId)
        {
            return UserRepo.GetById(userId);
        }

        public IUser? GetByName(string username)
        {
            return UserRepo.GetByName(username);
        }

        public bool IsEmailUnique(string email)
        {
            return UserRepo.IsEmailUnique(email);
        }

        public bool IsNameUnique(string username)
        {
            return UserRepo.IsNameUnique(username);
        }

        public void UpdatePassword(int userId, string PasswordHash)
        {
            var user = GetById(userId);
            if (user == null) throw new Exception("Entity not found");
            user.PasswordHash = PasswordHash;
            UserRepo.Save();
        }

        public bool IsEmailConfirmed(int userId)
        {
            var user = GetById(userId);
            if (user == null) return false;
            return user.EmailConfirmed;
        }

        public void SetEmailConfirmed(int userId)
        {
            var user = GetById(userId);
            if (user == null) throw new Exception("Entity not found");
            user.EmailConfirmed = true;
            UserRepo.Save();
        }
    }
}
