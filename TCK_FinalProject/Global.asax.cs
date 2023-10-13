using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace TCK_FinalProject
{
    public class MvcApplication : System.Web.HttpApplication
    {
        /*
            object sender: Đây là đối tượng đã kích hoạt sự kiện. Trong trường hợp của Application_Start, đối tượng kích hoạt sự
            kiện là ứng dụng ASP.NET MVC đang chạy.

            EventArgs e: Đây là một đối tượng chứa dữ liệu sự kiện. Trong trường hợp của Application_Start, e không chứa bất kỳ 
            dữ liệu nào vì sự kiện này không tạo ra dữ liệu.

            Trong trường hợp cụ thể của phương thức Application_Start, hai tham số này không được sử dụng, nhưng chúng vẫn phải 
            được bao gồm trong định nghĩa phương thức để tuân theo cấu trúc tiêu chuẩn của một sự kiện trong .NET. Phương thức 
            Application_Start được gọi một lần duy nhất khi ứng dụng khởi động, và không cần thông tin từ sender hoặc e.
        */
        protected void Application_Start(object sender, EventArgs e)
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
    }
}
