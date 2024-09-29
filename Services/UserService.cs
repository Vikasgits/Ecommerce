using E_commerce.Exceptions;
using E_commerce.Models;
using E_commerce.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;

namespace E_commerce.Services
{
    public class UserService : IUserService
    {
        private readonly IEntityRepository<User> _userRepository;

        public UserService(IEntityRepository<User> entityRepository)
        {
            _userRepository = entityRepository;
        }

        public bool Add(User user)

        {

            if (user == null) throw new InvalidUserException("Invalid User credentials");
            if (CheckValidity(user))
            {
                user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
                _userRepository.Add(user);
            }
            return false;
        }
       
        public List<User> GetAll()
        {
            var userList=_userRepository.Get().Include(user=>user.Orders).Where(users=>users.IsActive==true).ToList();///
            return userList;
        }

        public User GetByName(string name)
        {
            var user = _userRepository.Get().Where(user => user.Name== name).FirstOrDefault();
            if (user == null || user.IsActive==false) throw new UserNotFoundException("No user with such name exist");
            return user;
        }
        public User GetByEmail(string email)
        {
           var user= _userRepository.Get().Where(user=>user.Email == email).FirstOrDefault();
            if (user == null || user.IsActive == false) throw  new UserNotFoundException("No such user exist with given email");
            return user;

            
        }
        public User GetUserByEmail(string email) {
        return _userRepository.Get().Where(user => user.Email == email).FirstOrDefault();
        }

        public User GetByPhoneNumber(string phoneNumber)
        {
            var user = _userRepository.Get().Where(user => user.PhoneNumber == phoneNumber).FirstOrDefault();
            return user;
        }

        public User GetById(Guid id)
        {
            var user = _userRepository.Get(id);
            if (user == null)
            {
                throw new UserNotFoundException("No such User found to be deleted");
            }
            return user;
        }

        public User Update(User user)
        {
            if (user == null) throw new InvalidUserException("Invalid Update Credentials");
            var getUser=_userRepository.Get().AsNoTracking().Where(existUser=>existUser.UserId == user.UserId).FirstOrDefault();
            if (getUser == null ) throw new UserNotFoundException("No such user exists to be updated");
            var updatedUser=_userRepository.Update(user);
            return updatedUser;
        }

        public bool Delete(Guid id)
        {
            var getUser=GetById(id);
            
            return _userRepository.Delete(getUser);
            
        }

        public bool CheckValidity(User user) {
            foreach (User usr in _userRepository.Get()) {
                if (usr.PhoneNumber == user.PhoneNumber) {
                    throw new PhoneNumberAlreadyExistException("User with that number exist. Please provide a different number");
                }
                if (usr.Email == user.Email) {
                    throw new EmailAlreadyExistException(" Email is already registered. Use a different email");
               }
            }
            return true;
        }

       
    }
}
