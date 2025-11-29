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
        public void OnPost() 
        { 

        }
    }
}
