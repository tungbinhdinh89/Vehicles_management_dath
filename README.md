# VehicleCare

## Kiến trúc thư mục

Repository được tổ chức theo ranh giới triển khai và hướng tới Clean
Architecture:

```text
fe/                  # React + Vite frontend
  src/
  public/
  tests/
be/                  # ASP.NET Core Web API (sẽ triển khai sau)
  src/
  tests/
infra/               # Docker, PostgreSQL và cấu hình triển khai NAS (sẽ triển khai sau)
.github/workflows/   # GitHub Actions (sẽ triển khai sau)
```

Hiện tại chỉ `fe/` có mã nguồn chạy được. `be/` và `infra/` là các boundary
được chuẩn bị để không trộn mã ứng dụng với cấu hình hạ tầng. PostgreSQL dự kiến
chạy trong container Docker trên Synology NAS; backend ASP.NET Core API sẽ được
đặt trong `be/` và các workflow CI/CD sẽ đặt trong `.github/workflows/`.

Frontend prototype quản lý xe cá nhân bằng tiếng Việt, xây dựng với React 19,
TypeScript, Vite, Material UI Community + Icons, token Astra, React Router Data
Mode, TanStack Query, React Hook Form + Zod và Chart.js.

## Trải nghiệm

Trang tổng quan hiển thị dữ liệu minh họa. Chọn **Đăng nhập demo** để chỉnh sửa:

| Tài khoản | Email |
| --- | --- |
| Nguyễn Minh Anh | minhanh@vehiclecare.vn |
| Trần Hoàng Nam | hoangnam@vehiclecare.vn |

Mật khẩu dùng chung: `VehicleCare@123` (chỉ là thông tin demo, không phải bí mật).

- Hai tài khoản có dữ liệu nghiệp vụ tách biệt.
- Dữ liệu và phiên đăng nhập chỉ ở trong bộ nhớ; tải lại trang mất phiên và mọi
  thay đổi. Không dùng localStorage, sessionStorage, database, JWT hay backend.
- Đăng xuất không xóa dữ liệu trong bộ nhớ của tab; đăng nhập lại cùng tài khoản
  trong tab đó vẫn thấy thay đổi cho đến khi tải lại trang.
- Nhắc bảo dưỡng chỉ hiển thị trong ứng dụng khi web đang mở.
- Định dạng VNĐ, km, lít, dd/MM/yyyy; múi giờ Asia/Ho_Chi_Minh.

## Nghiệp vụ

- Quản lý nhiều ô tô/xe máy, ODO ban đầu và lịch sử ODO thủ công.
- Nhập, sửa, xóa nhiên liệu, dịch vụ và ODO; kiểm tra ODO với ngày trước/sau.
  Trong một ngày có thể có nhiều bản ghi với các ODO khác nhau.
- ODO hiện tại, báo cáo và chi phí được tính từ cùng một tập lịch sử.
- Kế hoạch đến hạn theo ngày hoặc km, tùy điều kiện nào đến trước.
- Hoàn tất kế hoạch bằng một dịch vụ có chi phí. Dịch vụ được liên kết với kế
  hoạch; không tạo thêm một khoản chi phí riêng.
- Kỳ kế tiếp được tính từ dịch vụ liên kết gần nhất. Sửa/xóa dịch vụ tự động
  tính lại kế hoạch; lịch cuối tháng được chặn về ngày hợp lệ cuối cùng.
- Lọc lịch sử, xem biểu đồ, xuất CSV theo bộ lọc, sửa hồ sơ và cấu hình nhắc nhở.

## Cấu trúc chính

- `fe/src/data.ts`: mô hình, mock API Promise và quy tắc nghiệp vụ.
- `fe/src/store.tsx`: phiên trong bộ nhớ, TanStack Query và thao tác dùng chung.
- `fe/src/app/routes.tsx`: cấu hình React Router.
- `fe/src/Layout.tsx`: điều hướng desktop/mobile và khung ứng dụng.
- `fe/src/pages.tsx`: các màn hình.
- `fe/src/Dialogs.tsx`: biểu mẫu có validation và xác nhận xóa.
- `fe/src/components.tsx`: biểu đồ, bảng lịch sử và component dùng chung.
- `fe/src/index.css`: phông chữ tự lưu trữ, token Astra tùy chỉnh navy/xanh dương.
- `fe/tests/data.test.ts`: kiểm thử các quy tắc nghiệp vụ.

## Kiểm tra

```sh
pnpm format fe/src fe/tests
pnpm exec tsc --noEmit
pnpm test
pnpm build
```

Server phát triển trong Figma Make đã được môi trường chạy sẵn. Không cần khởi
động thêm server. Nguồn và giấy phép tài nguyên tại `fe/public/ASSET-CREDITS.md`.
