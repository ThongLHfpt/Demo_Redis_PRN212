using System.Collections.Generic;
using DataAccessLayer.Entities;

namespace DataAccessLayer
{
    public interface IUserRepository
    {
        User GetById(int id);
        IEnumerable<User> GetAll();
        void Add(User entity);
        void Update(User entity);
        void Delete(User entity);
    }
}
