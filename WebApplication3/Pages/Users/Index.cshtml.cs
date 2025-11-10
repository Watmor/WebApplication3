using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;

namespace WebApplication3.Pages.Users
{
    public class IndexModel : PageModel
    {
        public List<User_Info> Users = new List<User_Info>();
        public void OnGet()
        {
            try
            {
                string connectionString = "Data Source=localhost\\sqlexpress;Initial Catalog=users;Integrated Security=True;Trust Server Certificate=True";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "SELECT * from Users";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                User_Info user = new User_Info();
                                // Fix: Id is an int property, assign the int value directly.
                                user.Id = reader.IsDBNull(0) ? 0 : reader.GetInt32(0);
                                user.Name = reader.IsDBNull(1) ? string.Empty : reader.GetString(1);
                                user.Email = reader.IsDBNull(2) ? string.Empty : reader.GetString(2);
                                user.Password = reader.IsDBNull(3) ? string.Empty : reader.GetString(3);

                                Users.Add(user);
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
    }
}
