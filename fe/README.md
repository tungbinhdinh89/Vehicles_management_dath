# Frontend

Frontend React + Vite của VehicleCare. Mã nguồn được tách riêng để sau này
frontend có thể được build và deploy độc lập với ASP.NET Core API.

Các lớp Clean Architecture dự kiến trong `src/`:

- `app/`: composition root, routing và cấu hình ứng dụng.
- `features/`: use case và UI theo từng nghiệp vụ.
- `domain/`: entity, value object và quy tắc nghiệp vụ thuần.
- `infrastructure/`: HTTP client, storage và adapter bên ngoài.
- `shared/`: component, hook và tiện ích dùng chung.

Các thư mục lớp sẽ được tạo khi có nghiệp vụ tương ứng; hiện trạng prototype
được giữ nguyên để không thay đổi hành vi.
