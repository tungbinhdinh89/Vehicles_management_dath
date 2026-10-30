# Backend changelog

## 2026-10-30 — Nền tảng đầu tiên

- Thêm solution .NET 10 gồm Domain/Application/Infrastructure/Api, UnitTests/IntegrationTests và DatabaseTool.
- Thêm env loader, PostgreSQL mappings, Identity/session/idempotency model, domain ODO/plan rules, migration và health/Scalar.
- Thêm bộ FR/ERD/contract/flow/development/testing; nối hướng dẫn agent từ root và be.
- Chưa triển khai auth/business APIs hoặc nối frontend. Kết quả chạy thực tế tại testing.md.
- Đã tạo/migrate vehiclecare trên PostgreSQL 18.6 và pass persistence smoke (cleanup fixture). 12 unit + 2 host tests pass; database Testcontainers bị chặn do không có Docker daemon.
- Nâng dependency có NuGet advisory lên bản vá; dùng console logging và deferred DI configuration/validate-on-start để HTTP host test không truy cập nhầm cấu hình database thật.
