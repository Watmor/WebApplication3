using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using WebApplication3.Pages.Users;

namespace WebApplication3.Pages.Users
{
    public class Sign_upModel : PageModel
    {
        public User_Info NewUser = new User_Info();
        public string Message = "";

        public void OnGet()
        {
        }

        public void OnPost()
        {
            NewUser.Name = Request.Form["name"];
            NewUser.Email = Request.Form["email"];
            NewUser.Password = Request.Form["password"];
            if (string.IsNullOrEmpty(NewUser.Name) || string.IsNullOrEmpty(NewUser.Email) || string.IsNullOrEmpty(NewUser.Password))
            {
                Message = "All fields are required!";
                return;
            }

            try
            {
                string connectionString = "Data Source=localhost\\sqlexpress;Initial Catalog=users;Integrated Security=True;Trust Server Certificate=True";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "INSERT INTO users " + "(Name, Email, Password) VALUES" + "(@Name, @Email, @Password)";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@Name", NewUser.Name);
                        command.Parameters.AddWithValue("@Email", NewUser.Email);
                        command.Parameters.AddWithValue("@Password", NewUser.Password);
                        int rowsAffected = command.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            Message = "User registered successfully!";
                        }
                        else
                        {
                            Message = "Error registering user.";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Message = "An error occurred: " + ex.Message;
                throw;
            }

            NewUser.Name = "";
            NewUser.Email = "";
            NewUser.Password = "";
            Response.Redirect("/Users/Signin");

        }
    }
}
