# VehicleCare — Tổng quan để tiếp quản dự án

> Cập nhật backend 2026-10-30: đã tạo nền tảng ASP.NET Core 10/EF Core/PostgreSQL.
> Đọc [doc/.be/README.md](doc/.be/README.md) để biết hiện trạng backend và kết quả kiểm chứng.
> Phần “không backend/database” bên dưới mô tả frontend prototype ban đầu; frontend hiện vẫn dùng mock, chưa tích hợp API nghiệp vụ.

> Tên file `sumary.md` được giữ đúng theo yêu cầu.
> Tài liệu mô tả mã nguồn hiện tại; phần chưa triển khai luôn được đánh dấu.
> Đọc tiếp [rules.md](./rules.md) để biết ràng buộc và [skill.md](./skill.md)
> để biết quy trình phát triển, thiết kế API và database.

## 1. Mục tiêu và trạng thái

VehicleCare là frontend prototype quản lý nhiều phương tiện cá nhân bằng tiếng
Việt: ô tô, xe máy, ODO thủ công, nhiên liệu, bảo dưỡng/sửa chữa, kế hoạch nhắc
hạn theo ngày/km và báo cáo chi phí.

- Hiện tại là ứng dụng React chạy trên Vite, **không có backend/database thật**.
- Dữ liệu nghiệp vụ, tài khoản demo và phiên đăng nhập nằm trong bộ nhớ của tab.
- Reload sẽ khởi tạo lại dữ liệu mẫu và mất phiên đăng nhập.
- Đăng xuất chỉ bỏ phiên; thay đổi trong kho mock vẫn còn đến khi reload.
- Không có đồng bộ giữa tab, trình duyệt, thiết bị hoặc người dùng thật.
- Nhắc bảo dưỡng là nội dung trong ứng dụng, không có tác vụ nền, email,
  SMS hay push notification.
- Định dạng hiển thị: tiếng Việt, VNĐ, km, lít, `dd/MM/yyyy`;
  múi giờ nghiệp vụ `Asia/Ho_Chi_Minh`.

**Ba tài liệu này không cài đặt hay kết nối backend.** Thiết kế backend trong
`skill.md` là đề xuất tương thích frontend, chưa chốt REST độc lập, Payload
trong Figma Make hay framework/database cụ thể.

## 2. Stack đang có

| Nhóm | Thư viện/công cụ |
| --- | --- |
| Nền tảng | React 19, React DOM 19, TypeScript strict, Vite 8 |
| Giao diện | Material UI Community 7, Material Icons, Emotion |
| Design system | Astra Make Kit; `@figma/astraui`, `@figma/astraui-kit` |
| CSS | Token CSS trong `src/index.css`, stylesheet Astra, Tailwind CSS v4 |
| Điều hướng | React Router 7, Data Mode với `createBrowserRouter` |
| Dữ liệu bất đồng bộ | TanStack Query 5 |
| Biểu mẫu | React Hook Form 7, Zod 4, `@hookform/resolvers` |
| Biểu đồ | Chart.js 4, `react-chartjs-2` |
| Kiểm thử | Node.js test runner, `node:assert/strict` |
| Công cụ | pnpm 10.34.3, Node.js 22 theo `.mise.toml`, oxfmt |

`package.json` và `pnpm-lock.yaml` là nguồn kiểm chứng dependency thực tế.
Không có Next.js, ORM, SDK database hoặc hệ thống xác thực thật trong ứng dụng.
Không thay MUI Community bằng gói Pro/Premium khi chưa có yêu cầu.

## 3. Bản đồ mã nguồn

| File/thư mục hiện có | Trách nhiệm |
| --- | --- |
| `AGENTS.md` | Hướng dẫn của repository; đọc trước khi sửa mã |
| `README.md` | Cách dùng demo, cấu trúc và lệnh kiểm tra |
| `src/main.tsx` | Import CSS chung, gắn React vào `#root`, bọc Astra `ThemeProvider` |
| `src/App.tsx` | MUI theme, `CssBaseline`, `QueryClientProvider`, `GarageProvider` |
| `src/app/App.tsx` | Render `RouterProvider` |
| `src/app/routes.tsx` | Cấu hình toàn bộ route |
| `src/Layout.tsx` | Sidebar, header, drawer mobile, tìm kiếm, trạng thái tải/lỗi, khung trang |
| `src/pages.tsx` | Dashboard, xe, chi tiết xe, lịch sử, kế hoạch, báo cáo, hồ sơ, cài đặt |
| `src/Dialogs.tsx` | Form xe/bản ghi/kế hoạch/đăng nhập và hộp thoại xác nhận xóa |
| `src/components.tsx` | Bộ lọc, thẻ xe, nhắc bảo dưỡng, bảng lịch sử, biểu đồ, trạng thái rỗng |
| `src/data.ts` | Kiểu dữ liệu, seed mock, mock API Promise, định dạng và hàm nghiệp vụ |
| `src/store.tsx` | Context `useGarage`, phiên, modal, bộ lọc, query/mutation, toast |
| `src/theme.ts` | Chuyển token CSS thành theme MUI |
| `src/index.css` | CSS entrypoint, khai báo font, token giao diện, layout và responsive |
| `tests/data.test.ts` | Kiểm thử mock API, ODO, chi phí và kỳ bảo dưỡng |
| `public/images/` | `car.jpg`, `scooter.jpg`, ảnh minh họa xe |
| `public/fonts/` | Be Vietnam Pro weight 400/500/600 và giấy phép OFL |
| `public/ASSET-CREDITS.md` | Nguồn và giấy phép tài nguyên |
| `vite.config.ts`, `tsconfig.json` | Cấu hình Vite, alias `@`, TypeScript |
| `.figma/make/` | Script do môi trường Make quản lý; không phải backend nghiệp vụ |

Không có `styles/global.css` trong cấu trúc hiện tại. CSS chung thực sự là
`src/index.css`; không tạo một nguồn token/font thứ hai chỉ để khớp tên cũ.

### Chuỗi khởi động

```text
index.html → src/main.tsx → src/App.tsx
  → Astra ThemeProvider + MUI ThemeProvider
  → QueryClientProvider → GarageProvider
  → src/app/App.tsx → src/app/routes.tsx
  → Layout → màn hình tương ứng
```

### Route hiện tại

| Đường dẫn | Màn hình/component |
| --- | --- |
| `/` | `Dashboard` |
| `/xe` | `Vehicles` |
| `/xe/:id` | `VehicleDetail` |
| `/nhien-lieu` | `Records`, loại `fuel` |
| `/bao-duong` | `Records`, loại `service` |
| `/ke-hoach` | `Plans` |
| `/nhac-bao-duong` | `Plans`, chế độ reminders |
| `/lich-su` | `Records`, tất cả loại |
| `/bao-cao` | `Reports` |
| `/ho-so` | `Profile` |
| `/cai-dat` | `Settings` |
| Đường dẫn không khớp | `NotFound` |

Đăng nhập là modal, không phải route `/login`. Tìm kiếm trên header dẫn vào
lịch sử qua query parameter `q`.

## 4. Mô hình dữ liệu frontend

Các kiểu sau được khai báo trong `src/data.ts`; đây là nguồn chuẩn hiện tại.

| Kiểu | Trường |
| --- | --- |
| `Account` | `id`, `name`, `email` |
| `Vehicle` | `id`, `name`, `plate`, `type`, `year`, `baseOdo`, `image` |
| `Entry` | `id`, `vehicleId`, `kind`, `title`, `date`, `odo`, `cost`, `liters`, `note`, `planId?` |
| `Plan` | `id`, `vehicleId`, `title`, `dueDate`, `dueOdo`, `intervalKm`, `intervalMonths` |
| `Garage` | `vehicles[]`, `entries[]`, `plans[]`, `profile`, `settings` |
| `profile` | `name`, `email`, `phone` |
| `settings` | `reminders`, `thresholdKm`, `thresholdDays` |

- `Vehicle.type` chỉ có `"Ô tô"` và `"Xe máy"`.
- `Entry.kind` chỉ có `"fuel"`, `"service"`, `"odo"`.
- Bảo dưỡng và sửa chữa cùng dùng loại `service`, chưa có taxonomy riêng.
- Ngày trong model là chuỗi `YYYY-MM-DD`, không phải chuỗi đã định dạng hiển thị.
- `cost`, `odo`, `liters` là số; không chứa ký hiệu tiền, đơn vị hoặc dấu phân cách.
- `planId` liên kết dịch vụ hoàn tất với kế hoạch; không phải một khoản chi mới.
- ODO hiện tại và trạng thái kế hoạch là dữ liệu suy ra, không phải trường lưu
  riêng trong `Vehicle`/`Plan`.
- Model hiện chưa có `userId`, `archivedAt`, version hay timestamp kiểm toán.

## 5. Dữ liệu mock nằm ở đâu?

Tất cả nằm trong `src/data.ts`, không có file JSON hoặc database ẩn.

| Thành phần | Vai trò |
| --- | --- |
| `accounts`, `demoPassword` | Hai tài khoản demo cố định, mật khẩu minh họa |
| `seed(account)` | Tạo xe, lịch sử, kế hoạch, hồ sơ và cài đặt cho từng tài khoản |
| `stores: Record<string, Garage>` | Kho trong bộ nhớ, khởi tạo khi module được nạp |
| `api.login(email, password)` | Xác nhận thông tin demo, trả `Account` |
| `api.get(id)` | Trả bản sao `Garage` của tài khoản |
| `api.get(null)` | Dữ liệu preview lấy từ kho của tài khoản đầu tiên |
| `api.update(id, action)` | Clone kho, chạy callback, chỉ thay kho nếu callback không ném lỗi |

Mock API giả lập độ trễ khoảng 350 ms; kết quả được `structuredClone` để tránh
component sửa trực tiếp kho.

- Nguyễn Minh Anh: `minhanh@vehiclecare.vn`, Toyota Vios và Honda Air Blade.
- Trần Hoàng Nam: `hoangnam@vehiclecare.vn`, Mazda 3.
- Mật khẩu demo được công khai trong `README.md` và màn hình đăng nhập;
  không dùng làm thông tin xác thực production.
- Mỗi xe có 24 bản ghi seed gồm nhiên liệu và dịch vụ; mỗi tài khoản có 3 kế hoạch.
  Ngày seed tương đối với ngày chạy, không phải bộ mốc ngày cố định.
- Một số ID kế hoạch như `p1` được dùng lại ở hai kho khác nhau.
  Nếu nhập seed vào database, phải ánh xạ theo cả tài khoản và ID cũ.
- Preview không phải một tài khoản khách được phân quyền trên server; đây chỉ
  là cách hiển thị dữ liệu minh họa trong prototype.

## 6. Luồng đọc/ghi hiện tại

```text
Màn hình → useGarage()
  đọc: useQuery(["garage", session?.id ?? "preview"]) → api.get(...)
  ghi: form → Zod/kiểm tra nghiệp vụ → save(callback)
       → useMutation → api.update(accountId, callback)
       → setQueryData(["garage", accountId], Garage mới) → render lại + toast
```

`save()` trả `Promise<boolean>`; form thường chỉ đóng khi lưu thành công.
Query dùng `staleTime: Infinity`; ứng dụng tắt refetch khi focus cửa sổ.
Đăng nhập/đăng xuất đặt lại bộ lọc xe và tháng.

**Điểm quan trọng khi thay mock:** callback `(garage) => void` là hàm JavaScript
chạy cùng tiến trình, không phải JSON có thể gửi qua HTTP. Không thể chỉ đổi
`api.update()` thành `fetch()` mà giữ nguyên mọi nơi gọi `save(callback)`.

## 7. Nghiệp vụ đã triển khai

- Xe: thêm/sửa/xóa, kiểm tra trùng biển số sau chuẩn hóa.
- ODO: thêm/sửa/xóa lịch sử; kiểm tra với ngày trước và sau; cho phép nhiều mức
  ODO trong cùng ngày. ODO hiện tại lấy lớn nhất giữa ODO ban đầu và lịch sử.
- Nhiên liệu: chi phí và số lít dương; dịch vụ: chi phí dương; ODO thuần không
  tạo chi phí. Form chuẩn hóa trường không áp dụng về 0.
- Kế hoạch: hiện yêu cầu cả ngày, ODO đến hạn và chu kỳ km/tháng.
- Hoàn tất: tạo một `Entry` loại `service` có `planId`, chi phí tính đúng một lần.
- Kỳ kế tiếp: tính từ bản ghi liên kết mới nhất theo ngày, rồi ODO; cộng tháng
  có chặn về ngày hợp lệ cuối tháng. Sửa/xóa dịch vụ làm kỳ hạn được tính lại.
- `planState()` trả `overdue`, `soon`, `good`; xem công thức và biên bằng 0
  trong `rules.md`, không tự diễn giải lại khi viết backend.
- Báo cáo: lọc xe/tháng, tổng nhiên liệu/dịch vụ, quãng đường ghi nhận, biểu đồ
  và CSV theo dữ liệu đã lọc ở màn hình báo cáo.
- Tắt nhắc nhở chỉ ẩn số nhắc trên điều hướng, không xóa kế hoạch hoặc lịch sử.

Validation hiện phân tán giữa Zod trong form, kiểm tra thủ công và
`validateEntry()`. `api.update()` **không tự kiểm tra toàn bộ mọi bất biến**;
không xem đây là tầng nghiệp vụ đủ an toàn để đưa lên production.

## 8. Khoảng trống và quyết định chưa chốt

| Hạng mục | Hiện trạng và hướng xử lý |
| --- | --- |
| Đăng ký trong phiên | Chưa có form/API đăng ký; test hiện chỉ chấp nhận hai tài khoản demo. Việc bỏ yêu cầu đăng ký trước đây chưa được người dùng xác nhận; phải hỏi khi triển khai phần này, không ghi thành quyết định đã chốt |
| Xe lưu trữ chỉ đọc | Là yêu cầu đã nêu nhưng chưa có field, giao diện hay chặn ghi. Không nhầm thao tác xóa xe hiện tại với lưu trữ |
| Backend/database thật | Chưa kết nối; chỉ có đề xuất trong `skill.md` |
| Xác thực/phân quyền thật | Chưa có; chặn modal trên client không bảo vệ được dữ liệu trên server |
| Nhắc nhở khi đóng web | Chưa có, không được mô tả là đã hoạt động |
| Đồng thời nhiều thiết bị | Mock không mô phỏng transaction/khóa giữa nhiều client |

Không tự sửa các khoảng trống trên khi nhiệm vụ chỉ là viết tài liệu.
Khi yêu cầu tiếp theo liên quan, đối chiếu `rules.md` và xác nhận các lựa chọn
còn ảnh hưởng đến phạm vi sản phẩm.

## 9. Kiểm tra và tiếp quản nhanh

1. Đọc `AGENTS.md` → tài liệu này → `rules.md` → phần liên quan trong `skill.md`.
2. Mở đúng nhóm file theo thay đổi; giữ cấu trúc hiện tại nếu chưa cần tách module.
3. Dùng lệnh kiểm tra ở `README.md` theo mức rủi ro; không khởi động thêm dev server.
4. Với thay đổi chỉ ở tài liệu, kiểm tra tên file, liên kết, nội dung khớp mã nguồn
   và `git diff --check`; không cần build/test ứng dụng.
5. Ghi rõ thay đổi đã thực hiện, kiểm tra thực sự đã chạy và phần chưa triển khai.
