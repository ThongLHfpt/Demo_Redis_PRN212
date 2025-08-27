## Hướng dẫn setup nhanh (Windows) và luồng chạy

### 1) Cài Redis nhanh trên Windows
1. Tải file `Redis-x64-3.0.504.zip` từ trang phát hành: [microsoftarchive/redis releases](https://github.com/microsoftarchive/redis/releases)
2. Giải nén ZIP 
3. chạy file
   redis-server.exe

   - Mặc định Redis sẽ lắng nghe tại `127.0.0.1:6379`.

Tuỳ chọn: Cài RedisInsight để quan sát dữ liệu

cài bản desktop từ trang RedisInsight, thêm một kết nối tới Redis server tại `localhost:6379` để quản lý và theo dõi dữ liệu.


### 2) Mô tả luồng code (caching với Redis)
- `WPF` gọi `BussinessLayer.UserService.GetUserProfile(userId)`.
- `UserService` tạo kết nối Redis tới `127.0.0.1:6379` bằng `StackExchange.Redis`( dùng để kết nối và tương tác với Redis database ) và lấy `IDatabase`.(interface cung cấp tất cả các method để tương tác với redis: StringGet()- lấy data từ cache, StringSet() - Lưu dữ liệu vào cache, Multiplexer.IsConnected - Kiểm tra kết nối)
- Tạo key cache dạng `user:{userId}` rồi kiểm tra:
  - Nếu có cache: trả về chuỗi "Cache hit | <ms> | <value>".
  - Nếu không có:
    1) Gọi `DataAccessLayer.UserRepository.GetById(userId)` để lấy user từ DB qua `Entity Framework Core`.
    2) Trong `UserRepository`, phương thức `ClearBufferPool()` được gọi trước mỗi truy vấn (chạy `CHECKPOINT; DBCC DROPCLEANBUFFERS;`) để loại bỏ các page được load vào buffer pool của sql server.
    3) Tạo chuỗi kết quả `User Name: {Name}, Age: {Age}` và ghi vào Redis với TTL 10 giây: `_cache.StringSet(cacheKey, value, TimeSpan.FromSeconds(10))`.
    4) Trả về chuỗi "DB Query | <ms> | <value>".
- Kết nối DB được cấu hình trong `WPF/appsettings.json`, đọc bởi `DemoDbContext` khi khởi tạo.

Ghi chú:
- Nếu Redis chưa chạy, code vẫn hoạt động nhưng sẽ bỏ qua cache (có `try/catch` an toàn quanh lệnh Redis).
- Bạn có thể theo dõi key sinh ra trong RedisInsight theo tiền tố `user:`.

