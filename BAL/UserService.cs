using DataAccessLayer;
using StackExchange.Redis;
using System.Diagnostics;

namespace BussinessLayer
{
    public class UserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IDatabase _cache;

        public UserService()
        {
            var context = new DataAccessLayer.Entities.DemoDbContext();
            _userRepository = new UserRepository(context);
            
            var options = ConfigurationOptions.Parse("127.0.0.1:6379");

            var multiplexer = ConnectionMultiplexer.Connect(options);
            _cache = multiplexer.GetDatabase();
        }

        public string GetUserProfile(int userId)
        {
            var cacheKey = $"user:{userId}";
            var stopwatch = Stopwatch.StartNew();

            if (_cache.Multiplexer.IsConnected)
            {
                try
                {
                    var cacheValue = _cache.StringGet(cacheKey);
                    if (cacheValue.HasValue)
                    {
                        stopwatch.Stop();
                        return $"Cache hit | {stopwatch.ElapsedMilliseconds} ms | {cacheValue}";
                    }
                }
                catch
                {
                    // Bỏ qua lỗi Redis để không làm hỏng luồng nghiệp vụ
                }
            }

            stopwatch.Restart();
            var user = _userRepository.GetById(userId);
            stopwatch.Stop();

            if (user is null)
            {
                return "Không tìm thấy user";
            }

            var dbValue = $"User Name: {user.Name}, Age: {user.Age}";
            try
            {
                _cache.StringSet(cacheKey, dbValue, TimeSpan.FromSeconds(10));
            }
            catch
            {
                // Bỏ qua lỗi Redis, chỉ không cache
            }
            return $"DB Query | {stopwatch.ElapsedMilliseconds} ms | {dbValue}";
        }
    }
}
