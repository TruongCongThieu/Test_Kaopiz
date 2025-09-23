# Test_Kaopiz
Các chức năng chính
1. Login page.
2. Register page, including the information: • Username/ Email • Password • Type of user
3. Home page, a blank page accessible only after successful login.
4. Remember me functionality to keep users logged in after successful login.
5. Logout functionality.

Hướng Dẫn Cấu Hình và Chạy Dự Án ASP.NET Core

1. Thiết lập Startup Project thành Web App
Để đảm bảo dự án chạy đúng với ứng dụng web (Web App), bạn cần thiết lập dự án web làm Startup Project trong Visual Studio. Làm theo các bước sau:

Mở Solution trong Visual Studio:

Mở Visual Studio và tải solution (.sln) của dự án.


Chọn Startup Project:

Trong Solution Explorer, tìm dự án web (thường có tên như YourProject.Web hoặc tên dự án chứa các file như Program.cs, Startup.cs hoặc appsettings.json).
Nhấp chuột phải vào dự án web > Chọn Set as Startup Project.

Kiểm tra cấu hình:

Nhấn F5 (hoặc nút Start trong Visual Studio) để kiểm tra xem dự án web có chạy không.
Nếu có nhiều dự án trong solution, đảm bảo rằng chỉ dự án web được chọn làm startup project.


2. Thay đổi chuỗi kết nối DefaultConnection
Chuỗi kết nối DefaultConnection được sử dụng để kết nối với cơ sở dữ liệu (thường là SQL Server). Bạn cần cập nhật chuỗi này trong file appsettings.json để trỏ tới cơ sở dữ liệu phù hợp.

Mở file appsettings.json hoặc appsettings.Development.json:

Trong Solution Explorer, tìm file appsettings.json hoặc appsettings.Development.json trong dự án web.

Mở file và tìm phần cấu hình kết nối, thường trông như sau:
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=YourDatabaseName;Trusted_Connection=True;MultipleActiveResultSets=true"
  }
}




Cập nhật chuỗi kết nối:

Thay đổi các thông số trong DefaultConnection:
Server: Tên hoặc địa chỉ của máy chủ SQL Server (ví dụ: localhost, .\SQLEXPRESS, hoặc IP của server).
Database: Tên cơ sở dữ liệu bạn muốn sử dụng (ví dụ: MyAppDb).
Trusted_Connection=True: Sử dụng xác thực Windows. Nếu dùng tài khoản SQL Server, thay bằng User Id=your_username;Password=your_password;.
Ví dụ chuỗi kết nối với SQL Server Authentication:"DefaultConnection": "Server=localhost;Database=MyAppDb;User Id=sa;Password=YourStrongPassword;MultipleActiveResultSets=true"


Lưu ý bảo mật:

Không commit chuỗi kết nối chứa thông tin nhạy cảm (như mật khẩu) vào Git. Sử dụng biến môi trường hoặc Azure Key Vault để lưu trữ an toàn.
Nếu cần, tạo file appsettings.Development.json cho môi trường phát triển và đặt chuỗi kết nối riêng.


Kiểm tra kết nối:

Sau khi cập nhật, đảm bảo cơ sở dữ liệu tồn tại và bạn có quyền truy cập (dùng SQL Server Management Studio hoặc công cụ tương tự để kiểm tra).




3. Chạy lệnh Update-Database cho Migrations
Dự án sử dụng Entity Framework Core để quản lý cơ sở dữ liệu qua migrations. Để áp dụng các migrations và cập nhật cơ sở dữ liệu, làm theo các bước sau:

Mở Package Manager Console:

Trong Visual Studio, đi tới Tools > NuGet Package Manager > Package Manager Console.


Chọn dự án chứa DbContext:

Trong Package Manager Console, đảm bảo bạn chọn đúng BusinessLogic.
Kiểm tra trong dropdown Default Project ở đầu cửa sổ console.


Chạy lệnh Update-Database:

Gõ lệnh sau và nhấn Enter:Update-Database


Lệnh này sẽ áp dụng tất cả các migrations chưa được áp dụng vào cơ sở dữ liệu được chỉ định trong DefaultConnection.


Xử lý lỗi:

Nếu gặp lỗi như "No DbContext was found", kiểm tra xem DbContext đã được cấu hình đúng trong dự án chưa.
Nếu lỗi kết nối, kiểm tra lại chuỗi DefaultConnection trong appsettings.json.
Đảm bảo đã cài Entity Framework Core tools:dotnet tool install --global dotnet-ef


Kiểm tra cơ sở dữ liệu:

Sau khi chạy lệnh, kiểm tra cơ sở dữ liệu (dùng SQL Server Management Studio hoặc công cụ tương tự) để đảm bảo các bảng đã được tạo.



4. Chạy dự án
Sau khi cấu hình startup project và cơ sở dữ liệu, bạn có thể chạy dự án:

Chạy qua Visual Studio:

Đảm bảo dự án web đã được chọn làm Startup Project.
Nhấn F5 (hoặc nút Start) để chạy dự án ở chế độ Debug.
Trình duyệt sẽ mở và tải ứng dụng (mặc định thường là https://localhost:5001 hoặc http://localhost:5000).


Chạy qua lệnh CLI:

Mở terminal (Command Prompt, PowerShell, hoặc Terminal trong VS Code).
Điều hướng đến thư mục của dự án web:cd path/to/YourProject.Web


Chạy lệnh:dotnet run


Terminal sẽ hiển thị URL của ứng dụng (ví dụ: Now listening on: https://localhost:5001).


Kiểm tra ứng dụng:

Mở trình duyệt và truy cập URL được cung cấp (ví dụ: https://localhost:5001).
Nếu gặp lỗi, kiểm tra:
Chuỗi kết nối DefaultConnection có đúng không.
Migrations đã được áp dụng thành công chưa.
Các gói NuGet cần thiết (như Microsoft.EntityFrameworkCore, Microsoft.AspNetCore.Mvc) đã được cài đặt.




Lưu ý khi chạy:

Nếu sử dụng HTTPS, đảm bảo đã cài đặt chứng chỉ phát triển:dotnet dev-certs https --trust


Nếu cần chạy ở môi trường cụ thể (như Development hoặc Production), đặt biến môi trường:export ASPNETCORE_ENVIRONMENT=Development  # Linux/Mac
set ASPNETCORE_ENVIRONMENT=Development     # Windows


Lưu ý bổ sung

Cài đặt môi trường:
Đảm bảo đã cài .NET SDK (phiên bản tương thích với dự án, ví dụ: .NET 6 hoặc .NET 8).
Cài Visual Studio (Community, Professional, hoặc Enterprise) với workload ASP.NET and web development.


Bảo mật:
Không commit appsettings.json chứa thông tin nhạy cảm vào Git. Sử dụng .gitignore hoặc biến môi trường.


Khắc phục sự cố:
Nếu gặp lỗi khi chạy Update-Database, kiểm tra log trong Package Manager Console hoặc chạy với tham số verbose:Update-Database -Verbose


Nếu dự án không chạy, kiểm tra file .csproj để đảm bảo các gói NuGet cần thiết đã được cài đặt.



Nếu bạn gặp vấn đề cụ thể hoặc cần thêm hướng dẫn, hãy liên hệ đội ngũ phát triển!