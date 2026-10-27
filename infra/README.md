# Infrastructure

Thư mục dành cho hạ tầng triển khai, tách khỏi mã nguồn frontend và backend.

Quy hoạch dự kiến:

```text
docker/
  postgres/                    # PostgreSQL container trên Synology NAS
  api/                         # container ASP.NET Core khi cần
compose/
  docker-compose.nas.yml       # stack cho DS220+
```

Secrets, volume dữ liệu và network sẽ được cấu hình qua biến môi trường hoặc
GitHub Actions secrets, không commit trực tiếp vào repository. Chưa tạo
container hoặc manifest triển khai theo phạm vi hiện tại.
