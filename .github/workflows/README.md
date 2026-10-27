# CI/CD

GitHub Actions sẽ được đặt tại thư mục này. Workflow dự kiến gồm:

1. CI: format, type-check, test và build `fe/`.
2. Backend CI: build và test solution ASP.NET Core trong `be/`.
3. CD: build image, publish artifact và deploy có kiểm soát lên Synology
   DS220+.

Chưa thêm workflow thực thi để không thay đổi pipeline hiện tại.
