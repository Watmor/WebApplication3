using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using static System.Reflection.Metadata.BlobBuilder;

namespace Bookstore.Pages.NewFolder
{

    public class ProductdetailModel : PageModel
    {

        public List<Book_Info> Books = new List<Book_Info>();
        public void OnGet()
        {

            try
            {
                string connectionString = "Data Source=localhost\\sqlexpress;Initial Catalog=users;Integrated Security=True;Trust Server Certificate=True";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "SELECT * FROM Books";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Book_Info book = new Book_Info();

                                book.BookID = reader.IsDBNull(0) ? 0 : reader.GetInt32(0);
                                book.Title = reader.IsDBNull(1) ? string.Empty : reader.GetString(1);
                                book.Author = reader.IsDBNull(2) ? string.Empty : reader.GetString(2);
                                book.Price = reader.IsDBNull(3) ? 0 : reader.GetDecimal(3);
                                book.Categories = reader.IsDBNull(4) ? string.Empty : reader.GetString(4);
                                book.Summary = reader.IsDBNull(5) ? string.Empty : reader.GetString(5);
                                book.PublishYear = reader.IsDBNull(6) ? 0 : reader.GetInt32(6);
                                book.ImageUrl = reader.IsDBNull(7) ? string.Empty : reader.GetString(7);

                                Books.Add(book);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
            }
        }

        public IActionResult OnPostAddToCart(int BookID, int Quantity)
        {
            try
            {
                // Lấy User ID từ session (khi login bạn phải set session này)
                int? userId = HttpContext.Session.GetInt32("UserID");

                Console.WriteLine($"UserID from session: {userId}");


                if (userId == null)
                {
                    // Nếu chưa login → chuyển về login
                    return RedirectToPage("/Users/Signin");
                }

                string connectionString = "Data Source=localhost\\sqlexpress;Initial Catalog=users;Integrated Security=True;Trust Server Certificate=True";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Kiểm tra xem sách này đã có trong giỏ chưa
                    string checkSql = "SELECT Quantity FROM Cart WHERE UserID=@uid AND BookID=@bid";
                    using (SqlCommand checkCmd = new SqlCommand(checkSql, connection))
                    {
                        checkCmd.Parameters.AddWithValue("@uid", userId);
                        checkCmd.Parameters.AddWithValue("@bid", BookID);

                        var result = checkCmd.ExecuteScalar();

                        if (result != null)
                        {
                            // Nếu đã có → cập nhật số lượng
                            string updateSql = @"UPDATE Cart 
                                         SET Quantity = Quantity + @qty
                                         WHERE UserID=@uid AND BookID=@bid";

                            using (SqlCommand updateCmd = new SqlCommand(updateSql, connection))
                            {
                                updateCmd.Parameters.AddWithValue("@uid", userId);
                                updateCmd.Parameters.AddWithValue("@bid", BookID);
                                updateCmd.Parameters.AddWithValue("@qty", Quantity);

                                updateCmd.ExecuteNonQuery();
                            }
                        }
                        else
                        {
                            // Nếu chưa có → thêm mới
                            string insertSql = @"INSERT INTO Cart(UserID, BookID, Quantity)
                                         VALUES(@uid, @bid, @qty)";

                            using (SqlCommand insertCmd = new SqlCommand(insertSql, connection))
                            {
                                insertCmd.Parameters.AddWithValue("@uid", userId);
                                insertCmd.Parameters.AddWithValue("@bid", BookID);
                                insertCmd.Parameters.AddWithValue("@qty", Quantity);

                                insertCmd.ExecuteNonQuery();
                            }
                        }
                    }
                }

                // Sau khi thêm giỏ hàng → chuyển đến trang Cart
                return RedirectToPage("/Payment/Cart");
            }
            catch (Exception ex)
            {
                Console.WriteLine("AddToCart Error: " + ex.Message);
                return Page();
            }
        }
        public void OnPost() 
        { 

        }
    }
}
