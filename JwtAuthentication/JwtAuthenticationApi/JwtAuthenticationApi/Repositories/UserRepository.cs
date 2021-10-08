using JwtAuthenticationApi.Models;
using Services.UserServ;
using System.Linq;

namespace JwtAuthenticationApi.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly JwtAuthContext db;

        public UserRepository(JwtAuthContext db)
        {
            this.db = db;
        }

        public void Delete(int userId)
        {
            var user = GetById(userId);
            db.Users.Remove(user as User);
        }

        public IUser? GetById(int userId)
        {
            return db.Users.Where(x => x.Id == userId).SingleOrDefault();
        }

        public IUser? GetByEmail(string email)
        {
            return db.Users.Where(x => x.Email == email).SingleOrDefault();
        }

        public IUser? GetByName(string username)
        {
            return db.Users.Where(x => x.UserName == username).SingleOrDefault();
        }

        public void Insert(IUser user)
        {
            db.Users.Add(user as User);
            db.SaveChanges();
        }

        public bool IsEmailUnique(string email)
        {
            return !db.Users.Where(x => x.Email == email).Any();
        }

        public bool IsNameUnique(string username)
        {
            return !db.Users.Where(x => x.UserName == username).Any();
        }

        public int Save()
        {
            return db.SaveChanges();
        }
    }
}
