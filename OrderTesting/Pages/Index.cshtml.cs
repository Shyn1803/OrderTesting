using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;
using System.Diagnostics;

namespace OrderTesting.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        public List<ProductInfo> goodsList = new List<ProductInfo>();
        
        [BindProperty(SupportsGet = true)]
        public string searchValue { get; set; }

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
            try
            {
                String connectionString = "Data Source=.\\SQLEXPRESS;Initial Catalog=mystore;Integrated Security=True";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    String sql = "SELECT * FROM chitiet_nhapxuat";

                    if (searchValue?.Length > 0)
                    {
                        sql = sql + " WHERE ma_vt LIKE '%" + searchValue + "%' OR ten_vt LIKE '%" + searchValue + "%'";
                    }

                    sql = sql + ";";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                ProductInfo productInfo = new ProductInfo();
                                productInfo.id = reader.GetInt32(0);
                                productInfo.so_phieu = reader.GetString(1);
                                productInfo.ngay_lap_phieu = reader.GetDateTime(2).ToString("dd/MM/yyyy");
                                productInfo.ma_vt = reader.GetString(3);
                                productInfo.ten_vt = reader.GetString(4);
                                productInfo.dvt = reader.GetString(5);
                                productInfo.sl_nhap = "" + reader.GetValue(6);
                                productInfo.sl_xuat = "" + reader.GetValue(7);

                                goodsList.Add(productInfo);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.Message);
            }

        }
    }

    public class ProductInfo
    {
        public int id;
        public string so_phieu;
        public string ngay_lap_phieu;
        public string ma_vt;
        public string ten_vt;
        public string dvt;
        public string? sl_nhap;
        public string? sl_xuat;
    }
}