# Backend

Backend dùng ASP.NET Core 10 Web API và được triển khai độc lập với frontend,
theo ranh giới Clean Architecture:

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

Nền tảng backend đã được tạo. Bắt đầu tại [hướng dẫn backend](../doc/.be/README.md)
và [hướng dẫn agent](AGENTS.md). API nghiệp vụ chưa triển khai; frontend vẫn là mock.

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File be/scripts/be.ps1 restore
powershell -NoProfile -ExecutionPolicy Bypass -File be/scripts/be.ps1 build
powershell -NoProfile -ExecutionPolicy Bypass -File be/scripts/be.ps1 test
powershell -NoProfile -ExecutionPolicy Bypass -File be/scripts/be.ps1 check
powershell -NoProfile -ExecutionPolicy Bypass -File be/scripts/be.ps1 migrate
powershell -NoProfile -ExecutionPolicy Bypass -File be/scripts/be.ps1 run
```

SDK .NET 10; copy `.env.example` thành `.env` và điền credential cục bộ.
Scalar: `http://localhost:5080/scalar/v1`; health: `/health/live`, `/health/ready`.
