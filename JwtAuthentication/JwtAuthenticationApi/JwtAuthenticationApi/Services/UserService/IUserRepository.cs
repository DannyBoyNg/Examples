namespace Services.UserServ
{
    public interface IUserRepository
    {
        void Delete(int userId);
        IUser? GetById(int userId);
        IUser? GetByEmail(string email);
        IUser? GetByName(string username);
        void Insert(IUser user);
        bool IsEmailUnique(string email);
        bool IsNameUnique(string username);
        int Save();
    }
}