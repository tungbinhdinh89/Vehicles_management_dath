# Quy trình agent backend

1. Đọc AGENTS gốc, `be/AGENTS.md`, README bộ này và trang liên quan; kiểm tra git status.
2. Đối chiếu FR/acceptance với rules.md và frontend; không coi mock client là bảo mật server.
3. Thêm use case theo feature vào Application; khai báo DTO/port tối thiểu, domain rules thuần C#.
4. Implement adapter Infrastructure và HTTP Api; luôn xét owner/archive/revision/idempotency/transaction.
5. Schema change: tạo migration, review SQL và kiểm thử PostgreSQL riêng. Không tự áp schema lúc startup.
6. Chạy kiểm thử tương ứng, ghi exit code/kết quả thực tế; không sửa test để che regression.
7. Cập nhật requirements/data/API/flow/architecture/testing nếu liên quan; luôn thêm changelog.
8. Review diff: không secret, không frontend thay ngoài scope, không artifact .tools/bin/obj; không tự commit/push/deploy.

## Cấu hình

`.env` là file key=value, hỗ trợ value có double quotes bao ngoài; không substitution/expansion/eval.
Script chỉ cho ConnectionStrings__VehicleCare, ASPNETCORE_ENVIRONMENT và ASPNETCORE_URLS.
Không dùng parser env này cho shell code. Env từ process có ưu tiên cao hơn file.
Thiếu connection string fail sớm; không fallback password trong source/appsettings.
SDK install ở `.tools` bị ignore; máy khác có thể dùng SDK hệ thống major 10.

## Database operations

- `check`: read-only xác thực và đọc version/TLS.
- `provision`: chỉ CREATE DATABASE vehiclecare nếu chưa tồn tại, dùng database postgres để kiểm tra catalog; cần quyền.
- `migrate`: chạy EF migration rõ ràng; xem schema hiện tại/backup nếu đã có dữ liệu.
- `smoke`: tạo fixture GUID riêng, commit, đọc bằng DbContext khác, cleanup đúng user fixture bằng cascade; không reset database.
- Nếu connection fail: kiểm tra route/firewall TCP 5433, Docker port mapping, pg_hba.conf, auth và quyền. Không tự thay NAS settings.
- Nếu CREATEDB bị từ chối: người quản trị chạy `CREATE DATABASE vehiclecare OWNER tungbinhdinh89;`, rồi chạy check/migrate.

## Tài liệu cũ

sumary/skill/rules ở gốc vẫn mô tả frontend prototype và yêu cầu. Chỉ dẫn mới về backend nằm bộ này.
Khi nối frontend thật sau này mới cập nhật nội dung “chỉ trong bộ nhớ” và thay mock; không đổi trước.
