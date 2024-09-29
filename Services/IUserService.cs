using E_commerce.Models;

namespace E_commerce.Services
{
    public interface IUserService
    {
        public List<User> GetAll();
        public User GetById(Guid id);
        public bool Add(User user);
        public User Update(User user);
        public User GetByEmail(string email);
        public User GetByName(string name);
        public User GetByPhoneNumber(string phoneNumber);
        public bool Delete(Guid id);
        public User GetUserByEmail(string email);
    }
}
