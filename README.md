# JiraSchedulingConnect
IDE: Sử dụng Rider hay Visual Studio đều được

## Restful Web API: (JiraSchedulingConnectAppService)

Luồng logic:
Models -> Services -> Controllers -> DTOs
- Common:
	- Các hằng số sẽ được khai báo trong Const. Lưu ý dùng readonly và viết hoa tên biến, phân tách bằng dấu "_"
	- Các hàm util sẽ được khai báo trong Utils
	
- Controller: chỉ làm nhiệm vụ mapping giữa Route với Logic code, Còn lại các luồng xử lý chính sẽ do Service đảm nhiệm. Không viết gì hơn ngoài việc call service và mapping exception ở trong Controller

- Service:
	- Các xử lý của thuật toán và các class liên quan sẽ được tạo ra ở 

- DTO:
	- Luôn sử dụng DTO trong việc tương tác với API, cả lúc lấy từ request lẫn lúc trả response
	- Dùng AutoMapper để mapping giữa Model và DTO, khai báo các mapper trong Constructor của AutoMapperProfile.
	
## Algorithm Class Lib: (RcpspAlgorithmLibrary)
Project để thực hiện việc xử lý thuật toán.
Tất cả các code logic của việc chạy thuật toán đều được xử lý ở đây, Sau đó sẽ trả về kết quả cuối cùng cho phía Web Api project.

## Console App: (RcpspAlgorithmConsole)
Thực hiện việc test thuật toán nhanh hơn bằng ứng dụng Console

(ClassLib đã được setup reference tới 2 project web api và console nên có thể được sử dụng như thông thường)