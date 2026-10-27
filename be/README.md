# Backend

Backend sẽ dùng ASP.NET Core Web API và được triển khai độc lập với frontend.
Khi bắt đầu triển khai, giữ ranh giới Clean Architecture:

```text
src/
  VehicleCare.Api/             # HTTP endpoints, middleware, composition root
  VehicleCare.Application/     # use cases, ports, DTOs
  VehicleCare.Domain/          # entities, value objects, domain rules
  VehicleCare.Infrastructure/  # EF Core, PostgreSQL, external adapters
tests/
  VehicleCare.UnitTests/
  VehicleCare.IntegrationTests/
```

Chưa tạo project hoặc kết nối database theo phạm vi hiện tại.
