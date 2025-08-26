using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using DataAccessLayer.Entities;

namespace DataAccessLayer
{
    public class UserRepository : IUserRepository
    {
        private readonly DemoDbContext _context;
        private readonly DbSet<User> _dbSet;
        

        public UserRepository(DemoDbContext context, int simulatedLatencyMs = 0)
        {
            _context = context;
            _dbSet = context.Users;
            
        }

        public void ClearBufferPool()
        {
            var conn = _context.Database.GetDbConnection();
            using (var command = conn.CreateCommand())
            {
                if (conn.State != System.Data.ConnectionState.Open)
                    conn.Open();

                command.CommandText = "CHECKPOINT; DBCC DROPCLEANBUFFERS;";
                command.ExecuteNonQuery();
            }
        }

        public User GetById(int id)
        {
            ClearBufferPool(); // clear trước mỗi query
            return _dbSet.FirstOrDefault(u => u.Id == id);
        }

        public IEnumerable<User> GetAll()
        {
            return _dbSet.ToList();
        }

        public void Add(User entity)
        {
            _dbSet.Add(entity);
            _context.SaveChanges();
        }

        public void Update(User entity)
        {
            _dbSet.Update(entity);
            _context.SaveChanges();
        }

        public void Delete(User entity)
        {
            _dbSet.Remove(entity);
            _context.SaveChanges();
        }
    }
}
