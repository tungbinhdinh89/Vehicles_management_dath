# Hướng dẫn backend VehicleCare

ASP.NET Core 10 monolith, Clean Architecture, PostgreSQL và Scalar.
Agent chuyên backend phải đọc `be/AGENTS.md` trước khi sửa.

## Thứ tự đọc

1. [Kiến trúc/package](architecture.md).
2. [Yêu cầu/trạng thái](requirements.md).
3. [Entity/ERD](data-model.md).
4. [API dự kiến](api-contracts.md).
5. [Flow tính năng](functional-flows.md).
6. [Phát triển](development-guide.md), [kiểm thử](testing.md), [changelog](changelog.md).

## Hiện trạng

Đã có project, model/mapping, domain rules ODO/kế hoạch, migration, health endpoints và công cụ database.
Kết quả chạy thực tế tại [testing.md](testing.md).
Identity/session/idempotency mới là model lưu trữ; chưa có auth hoặc API nghiệp vụ.
Frontend vẫn dùng mock, không được mô tả đã lưu PostgreSQL.

## Cấu hình và chạy

Cần SDK .NET 10; `be/global.json` cho phép feature version ổn định mới hơn trong major 10.
Script ưu tiên SDK riêng tại `be/.tools/dotnet`, nếu không có dùng PATH.
Copy `be/.env.example` thành `be/.env`, đặt credential cục bộ. `.env` bị Git ignore.
Process environment ưu tiên hơn `.env`; script khôi phục environment sau khi chạy.

```powershell
# Chạy từ root repo; ExecutionPolicy chỉ áp dụng process này.
powershell -NoProfile -ExecutionPolicy Bypass -File be/scripts/be.ps1 restore
powershell -NoProfile -ExecutionPolicy Bypass -File be/scripts/be.ps1 build
powershell -NoProfile -ExecutionPolicy Bypass -File be/scripts/be.ps1 test
powershell -NoProfile -ExecutionPolicy Bypass -File be/scripts/be.ps1 check
# Provision chỉ tạo database chưa có, cần CREATEDB; không sửa database đã tồn tại.
powershell -NoProfile -ExecutionPolicy Bypass -File be/scripts/be.ps1 provision
powershell -NoProfile -ExecutionPolicy Bypass -File be/scripts/be.ps1 migrate
powershell -NoProfile -ExecutionPolicy Bypass -File be/scripts/be.ps1 smoke
powershell -NoProfile -ExecutionPolicy Bypass -File be/scripts/be.ps1 run
```

Đích LAN: `192.168.1.200:5433`, database `vehiclecare`.
`ConnectionStrings__VehicleCare` ánh xạ `ConnectionStrings:VehicleCare` ở runtime và EF design-time.
Không commit secret, không dùng `VITE_*`, không bật sensitive data logging.
Không tự migrate/seed lúc startup. Không đổi Docker container/volume NAS.
`check` in database/version/TLS thực tế, không in credential.
Không ép SSL Disable hoặc bỏ kiểm tra certificate; production cần TLS theo hạ tầng.

`/health/live` kiểm tra process; `/health/ready` kiểm tra kết nối (không xác nhận schema).
Scalar `/scalar/v1`, OpenAPI `/openapi/v1.json` chỉ có trong Development.

## Cập nhật tài liệu

Mỗi thay đổi backend phải cập nhật trang liên quan và changelog cùng thay đổi.
Không gắn nhãn đã triển khai cho thiết kế chưa có code; không ghi test pass khi chưa chạy.
