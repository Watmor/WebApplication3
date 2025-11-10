using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System.ComponentModel.DataAnnotations;

namespace WebApplication3.Pages
{
    public class LoginModel : PageModel
    {

        [BindProperty]
        public LoginInput Input { get; set; } = new();

        public string ErrorMessage { get; set; } = string.Empty;

        public void OnGet()
        {
            ErrorMessage = string.Empty;
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                ErrorMessage = "Please provide both email and password.";
                return Page();
            }

            // Use the same connection string pattern as other pages in the project.
            string connectionString = "Data Source=localhost\\sqlexpress;Initial Catalog=users;Integrated Security=True;Trust Server Certificate=True";

            try
            {
                using var connection = new SqlConnection(connectionString);
                connection.Open();

                // Parameterized query to avoid SQL injection.
                string sql = "SELECT Id, Name, Email FROM User_Info WHERE Email = @Email AND Password = @Password";
                using var command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@Email", Input.Email ?? string.Empty);
                command.Parameters.AddWithValue("@Password", Input.Password ?? string.Empty);

                using var reader = command.ExecuteReader();
                if (reader.Read())
                {
                    // Login successful. Redirect to the users index page.
                    // You can set a cookie or TempData here if you later add auth/session support.
                    return RedirectToPage("/Users/Index");
                }
                else
                {
                    ErrorMessage = "Invalid email or password.";
                    return Page();
                }
            }
            catch (Exception ex)
            {
                // Log the exception in a real app. For now show a friendly message.
                ErrorMessage = "An error occurred while attempting to sign in.";
#if DEBUG
                // Include exception details in development to help debugging
                ErrorMessage += " " + ex.Message;
#endif
                return Page();
            }
        }

        public class LoginInput
        {
            [Required]
            [Display(Name = "Email")]
            public string? Email { get; set; }

            [Required]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string? Password { get; set; }
        }
    }
}
