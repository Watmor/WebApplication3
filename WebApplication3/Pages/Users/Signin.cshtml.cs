using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;



namespace WebApplication3.Pages.Users
{
    public class SigninModel : PageModel
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
                string sql = "SELECT * FROM users WHERE Email = @Email AND Password = @Password";
                using var command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@Email", Input.Email ?? string.Empty);
                command.Parameters.AddWithValue("@Password", Input.Password ?? string.Empty);

                using var reader = command.ExecuteReader();
                if (reader.Read())
                {
                    // Login successful. Redirect to the users index page.
                    // You can set a cookie or TempData here if you later add auth/session support.
                    return RedirectToPage("Index");
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



        public async Task<IActionResult> OnGetGoogleLogin()
        {
            var props = new AuthenticationProperties
            {
                RedirectUri = Url.Page("/Users/Signin", "GoogleResponse")
            };

            return Challenge(props, GoogleDefaults.AuthenticationScheme);
        }

        
        public async Task<IActionResult> OnGetGoogleResponse()
        {
            var result = await HttpContext.AuthenticateAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);

            var claims = result.Principal.Identities
                .FirstOrDefault()?.Claims.Select(claim => new
                {
                    claim.Issuer,
                    claim.OriginalIssuer,
                    claim.Type,
                    claim.Value
                });

            string UserEmail = claims.FirstOrDefault(c => c.Type == "email")?.Value;
            string UserName = claims.FirstOrDefault(c => c.Type == "name")?.Value;

            // Debug: In ra tất cả các claim
            foreach (var claim in result.Principal.Claims)
                Console.WriteLine($"{claim.Type} : {claim.Value}");
           
            AddUserInfo newUser = new AddUserInfo();
            newUser.Load(HttpContext);


            //    //{


            return RedirectToPage("/Users/Index");
        }

        //public async Task<IActionResult> OnGetGoogleResponse()
        //{
        //    var result = await HttpContext.AuthenticateAsync(
        //        CookieAuthenticationDefaults.AuthenticationScheme);

        //    var claims = result.Principal.Identities
        //        .FirstOrDefault()?.Claims;

        //    string? email = claims.FirstOrDefault(c => c.Type == "email")?.Value;
        //    string? name = claims.FirstOrDefault(c => c.Type == "name")?.Value;

        //    // Debug: In ra tất cả các claim
        //    //foreach (var claim in result.Principal.Claims)
        //    //    Console.WriteLine($"{claim.Type} : {claim.Value}");
        //    //}


        //    //try
        //    //{
        //    //    string connectionString = "Data Source=localhost\\sqlexpress;Initial Catalog=users;Integrated Security=True;Trust Server Certificate=True";

        //    //    using (SqlConnection connection = new SqlConnection(connectionString))
        //    //    {
        //    //        connection.Open();

        //    //        // Kiểm tra user đã tồn tại chưa
        //    //        string checkSql = "SELECT COUNT(*) FROM users WHERE Email = @Email";
        //    //        using (SqlCommand checkCmd = new SqlCommand(checkSql, connection))
        //    //        {
        //    //            string insertSql = "INSERT INTO users " + "(Name, Email, Password) VALUES" + "(@Name, @Email, @Password)";
        //    //            using (SqlCommand insertCmd = new SqlCommand(insertSql, connection))
        //    //            {
        //    //                insertCmd.Parameters.AddWithValue("@Name", name ?? "");
        //    //                insertCmd.Parameters.AddWithValue("@Email", email ?? "");

        //    //                insertCmd.Parameters.AddWithValue("@Password", "GoogleOAuth2");
        //    //                insertCmd.ExecuteNonQuery();
        //    //            }
        //    //        }
        //    //    }
        //    //}
        //    //catch (Exception ex)
        //    //{
        //    //    // Lỗi database → redirect về signin
        //    //    return RedirectToPage("/Index");
        //    //}





        //    // Redirect sau khi login thành công
        //    return RedirectToPage("/Index");
        //}


        public class AddUserInfo
        {
            public string UserName { get; private set; }
            public string UserEmail { get; private set; }

            public void Load(HttpContext context)
            {
                if (context.User.Identity != null && context.User.Identity.IsAuthenticated)
                {
                    UserName = context.User.Identity.Name;
                    UserEmail = context.User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress")?.Value;

                    // Debug: kiểm tra giá trị
                    Console.WriteLine($"UserName: {UserName}");
                    Console.WriteLine($"UserEmail: {UserEmail}");
                }

                try
                {
                    string connectionString = "Data Source=localhost\\sqlexpress;Initial Catalog=users;Integrated Security=True;Trust Server Certificate=True";
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        // Kiểm tra user đã tồn tại chưa
                        string checkSql = "SELECT COUNT(*) FROM users WHERE Email = @Email";
                        using (SqlCommand checkCmd = new SqlCommand(checkSql, connection))
                        {
                            checkCmd.Parameters.AddWithValue("@Email", UserEmail ?? "");
                            int userCount = (int)checkCmd.ExecuteScalar();
                            if (userCount == 0)
                            {
                                // Nếu chưa tồn tại, thêm user mới
                                string insertSql = "INSERT INTO users " + "(Name, Email, Password) VALUES" + "(@Name, @Email, @Password)";
                                using (SqlCommand insertCmd = new SqlCommand(insertSql, connection))
                                {
                                    insertCmd.Parameters.AddWithValue("@Name", UserName ?? "");
                                    insertCmd.Parameters.AddWithValue("@Email", UserEmail ?? "");
                                    insertCmd.Parameters.AddWithValue("@Password", "GoogleOAuth2");
                                    insertCmd.ExecuteNonQuery();
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Lỗi database → không làm gì cả
                    Console.WriteLine($"Lỗi: {ex.Message}");
                    Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                }


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
