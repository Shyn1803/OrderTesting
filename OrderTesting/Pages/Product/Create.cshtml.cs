using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Primitives;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Reflection.Emit;

namespace OrderTesting.Pages.Product
{
    public class CreateModel : PageModel
    {
        public ProductInfo productInfo = new ProductInfo();
        public String soPhieuHelper = "";
        public String ngayLapPhieuHelper = "";
        public String maVatTuHelper = "";
        public String tenVatTuHelper = "";
        public String donViTinhHelper = "";
        public String errorMessage = "";
        public String successMessage = "";

        public void OnGet()
        {
        }

        private static Boolean HasValue(String value)
        {   
            if (value.Length > 0)
            {
                return true;
            }

            return false;
        }

        private static String ErrorMessageHelperField(String field)
        {
            String message = "Trường " + field + " là bắt buộc!";

            return message;
        }

        public void OnPost()
        {
            productInfo.so_phieu = Request.Form["soPhieu"];
            productInfo.ngay_lap_phieu = Request.Form["ngayLapPhieu"];
            productInfo.ma_vt = Request.Form["maVatTu"];
            productInfo.ten_vt = Request.Form["tenVatTu"];
            productInfo.dvt = Request.Form["donViTinh"];
            productInfo.sl_nhap = StringValues.IsNullOrEmpty(Request.Form["soLuongNhap"]) ? "" : Request.Form["soLuongNhap"];
            productInfo.sl_xuat = StringValues.IsNullOrEmpty(Request.Form["soLuongXuat"]) ? "" : Request.Form["soLuongXuat"];

            soPhieuHelper = HasValue(productInfo.so_phieu) ? "" : ErrorMessageHelperField("số phiếu");
            ngayLapPhieuHelper = HasValue(productInfo.ngay_lap_phieu) ? "" : ErrorMessageHelperField("ngày lập phiếu");
            maVatTuHelper = HasValue(productInfo.ma_vt) ? "" : ErrorMessageHelperField("mã vật tư");
            tenVatTuHelper = HasValue(productInfo.ten_vt) ? "" : ErrorMessageHelperField("tên vật tư");
            donViTinhHelper = HasValue(productInfo.dvt) ? "" : ErrorMessageHelperField("đơn vị tính");

            if (!HasValue(productInfo.so_phieu) || !HasValue(productInfo.ngay_lap_phieu) || !HasValue(productInfo.ma_vt) || !HasValue(productInfo.ten_vt) || !HasValue(productInfo.dvt))
            {
                return;
            }

            // Save the new product into DB
            try
            {
                String connectionString = "Data Source=.\\SQLEXPRESS;Initial Catalog=mystore;Integrated Security=True";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    String sql = "INSERT INTO chitiet_nhapxuat ";
                    String queryColumns = "(so_phieu, ngay_lap_phieu, ma_vt, ten_vt, dvt";
                    String queryParams = "(@so_phieu, @ngay_lap_phieu, @ma_vt, @ten_vt, @dvt";
                    if (productInfo.sl_nhap.Length > 0)
                    {
                        queryColumns = queryColumns + ", sl_nhap";
                        queryParams = queryParams + ", @sl_nhap";
                    }

                    if (productInfo.sl_xuat.Length > 0)
                    {
                        queryColumns = queryColumns + ", sl_xuat";
                        queryParams = queryParams + ", @sl_xuat";
                    }
                    queryColumns = queryColumns + ") ";
                    queryParams = queryParams + ");";

                    sql = sql + queryColumns + "VALUES " + queryParams;

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@so_phieu", productInfo.so_phieu);
                        command.Parameters.AddWithValue("@ngay_lap_phieu", productInfo.ngay_lap_phieu);
                        command.Parameters.AddWithValue("@ma_vt", productInfo.ma_vt);
                        command.Parameters.AddWithValue("@ten_vt", productInfo.ten_vt);
                        command.Parameters.AddWithValue("@dvt", productInfo.dvt);
                        if (productInfo.sl_nhap.Length > 0)
                        {
                            command.Parameters.AddWithValue("@sl_nhap", productInfo.sl_nhap.Length > 0 ? float.Parse(productInfo.sl_nhap) : null);
                        }
                        if (productInfo.sl_xuat.Length > 0)
                        {
                            command.Parameters.AddWithValue("@sl_xuat", productInfo.sl_xuat.Length > 0 ? float.Parse(productInfo.sl_xuat) : null);
                        }

                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                errorMessage = ex.Message;
                return;
            }

            Response.Redirect("/");

        }
    }
}
